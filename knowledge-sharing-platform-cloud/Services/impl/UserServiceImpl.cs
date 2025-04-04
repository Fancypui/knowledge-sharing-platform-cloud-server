using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.DTO;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.UserReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.UserResp;
using knowledge_sharing_platform_cloud.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Stripe;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class UserServiceImpl : IUserService
    {
        private readonly UserRepo _userRepo;
        private readonly ChannelRepo _channelRepo;
        private readonly ChannelMemberRepo _channelMemberRepo;

        private readonly ChannelSummaryCache _channelSummaryCache;
        private readonly UserCache _userCache;

        public UserServiceImpl(
            UserRepo userRepo,
            ChannelRepo channelRepo,
            ChannelMemberRepo channelMemberRepo,
            ChannelSummaryCache channelSummaryCache,
            UserCache userCache)
        {
            _userRepo = userRepo;
            _channelRepo = channelRepo;
            _channelMemberRepo = channelMemberRepo;

            _channelSummaryCache = channelSummaryCache;
            _userCache = userCache;
        }

        public async Task<CreateUserResp> CreateUser(CreateUserReq createUserReq)
        {
            User user = new()
            {
                Email = createUserReq.Email,
                Password = createUserReq.Password,
                Username = createUserReq.Username,
                Description = createUserReq.Description,
                ProfileUrl = createUserReq.ProfileUrl
            };

            User newUser = await _userRepo.CreateUserAsync(user);

            if (newUser == null) 
            {
                throw new BusinessException("Failed to create user.");
            }
            string jwtToken = JWTHelper.IssueToken(newUser.Id);
            CreateUserResp response = new()
            {
                UserId = newUser.Id,
                Token = jwtToken,
            };

            return response;
        }

        public async Task<IEnumerable<UserJoinedChannelListResp>> UserJoinedChannelList(long uid)
        {
            /**
             * retreive all channel that user has joinned
             */
            IEnumerable<long> joinedChannelIdList = await _channelMemberRepo.GetUserJoinedChannels(uid);
            /**
             * extract channel summary info
             */
            var channelSummaryDTOMap = await _channelSummaryCache.GetBatch(joinedChannelIdList.ToList());

            var channelOwnerIdList = channelSummaryDTOMap.Values
                .Where(c=>c!=null)
                .Select(c => c.ChannelOwnerId)
                .ToList();

            var channelOwnerUserInfoMap = await _userCache.GetBatch(channelOwnerIdList);

            IEnumerable<UserJoinedChannelListResp> response = channelSummaryDTOMap
                .Where(channelSummaryDTO => channelSummaryDTO.Value!=null)
                .Select(channelSummaryDTO =>
                {
                    ChannelSummaryDTO channelSummary = channelSummaryDTO.Value;
                    var channelOwner = channelOwnerUserInfoMap.GetValueOrDefault(channelSummary.ChannelOwnerId,null);

                    string channelOwnerName = channelOwner?.Username??"Unknown Owner";
                    string channelOwnerProfileUrl = channelOwner?.ProfileUrl??"Unknown Profile Url";

                    return new UserJoinedChannelListResp
                    {
                        ChannelId = channelSummaryDTO.Key,
                        ChannelTopic = channelSummary.Topic,
                        ChannelImgUrl = channelSummary.ChannelImgUrl,
                        ChannelBackgroundUrl = channelSummary.ChannelImgBackground,
                        ChannelOwnerName = channelOwnerName,
                        ChannelOwnerProfileUrl = channelOwnerProfileUrl,
                    };
                }
                );

            return response;
        }


        public async Task<IEnumerable<UserManagedChannelListResp>> UserManagedChannelList(long uid)
        {
            IEnumerable<Channel> userChannels = await _channelRepo.GetChannelByUserId(uid);

            if (userChannels == null|| !userChannels.Any())
            {
                throw new BusinessException("Fail to get user managed channels");
            }

            IEnumerable<UserManagedChannelListResp> response = userChannels.Select(channel =>
            {
                return new UserManagedChannelListResp()
                {
                    Topic = channel.Topic,
                    Description = channel.Description,
                    ImageUrl = channel.ChannelImgUrl,
                    ChannelBackgroundUrl = channel.ChannelImgBackground,
                    ChannelId=channel.Id
                };
            });

            return response;
        }

        public async Task<UserRegisterResp> userRegistration(UserRegisterReq request)
        {
            // Validate email format
            if (string.IsNullOrWhiteSpace(request.Email) || !EmailHelper.IsValidEmail(request.Email))
            {
                throw new BusinessException("Invalid Email");
            }
            // Validate password length
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 5)
            {
                throw new BusinessException("Invalid Password Length");
            }
            // Validate username length
            if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 3)
            {
                throw new BusinessException("Length of Username must be a minimum of 3");
            }

            byte[] hash, salt;
            PasswordHasher.CreatePasswordHash(request.Password, out hash, out salt);
            string dbHash = Convert.ToBase64String(hash);
            string dbSalt = Convert.ToBase64String(salt);

            User newUser = new User
            {
                Username = request.Username,
                Password = dbHash,
                Salt = dbSalt,
                Email = request.Email,
            };
            try
            {
                newUser = await _userRepo.SaveNewUser(newUser);
                
            }
            catch (DbUpdateException ex)
            {
                // Check if the inner exception is related to a unique constraint
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627) // SQL Server unique constraint violation
                {
                    throw new BusinessException("Email already exists.");
                }
                throw;
            }
            var jwtToken = JWTHelper.IssueToken(newUser.Id);
            return new UserRegisterResp
            {
                Token = jwtToken
            };


        }
        public async Task<UserLogInResp> userLogIn(UserLogInReq request)
        {
            // Validate email format
            if (string.IsNullOrWhiteSpace(request.Email) || !EmailHelper.IsValidEmail(request.Email))
            {
                throw new BusinessException("Invalid Email");
            }
            // Validate password length
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 5)
            {
                throw new BusinessException("Invalid Password Length");
            }
            var user = await _userRepo.getByEmail(request.Email);
            if (user == null)
            {
                throw new BusinessException("Invalid email or password.");
            }
            byte[] passwordHash = Convert.FromBase64String(user.Password);
            byte[] saltHash = Convert.FromBase64String(user.Salt);
            bool isValidPassword = PasswordHasher.VerifyPasswordHash(request.Password,passwordHash, saltHash);
            if (!isValidPassword)
            {
                throw new BusinessException("Invalid email or password.");
            }
            string token = JWTHelper.IssueToken(user.Id);
            return new UserLogInResp
            {
                Token = token
            };
        }

        public async Task<UserInfoResp> GetUserInfo(long uid)
        {
            var user = await  _userCache.Get(uid);
            if (user == null)
            {
                throw new BusinessException("user not found");
            }
            return new UserInfoResp
            {
                UserId = user.Id,
                UserName = user.Username,
                Description = user.Description
            };
        }
    }
}
