using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Models.Channel;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.DTO;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.UserReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.UserResp;
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

            CreateUserResp response = new()
            {
                UserId = newUser.Id
            };

            return response;
        }

        public async Task<IEnumerable<UserJoinedChannelListResp>> UserJoinedChannelList(UserJoinedChannelListReq userJoinedChannelListReq)
        {
            IEnumerable<long> joinedChannelIdList = await _channelMemberRepo.GetUserJoinedChannels(userJoinedChannelListReq.UserId);

            var channelSummaryDTOMap = await _channelSummaryCache.GetBatch(joinedChannelIdList.ToList());

            var channelOwnerIdList = channelSummaryDTOMap.Values.Select(c => c.ChannelOwnerId).ToList();

            var channelOwnerUserInfoMap = await _userCache.GetBatch(channelOwnerIdList);

            IEnumerable<UserJoinedChannelListResp> response = channelSummaryDTOMap.Select(channelSummaryDTO =>
            {
                ChannelSummaryDTO channelSummary = channelSummaryDTO.Value;
                User channelOwner = channelOwnerUserInfoMap.GetValueOrDefault(channelSummary.ChannelOwnerId);

                string channelOwnerName = channelOwner.Username;
                string channelOwnerProfileUrl = channelOwner.ProfileUrl;

                return new UserJoinedChannelListResp
                {
                    ChannelId = channelSummaryDTO.Key,
                    ChannelTopic = channelSummary.Topic,
                    ChannelImgBackground = channelSummary.ChannelImgBackground,
                    ChannelOwnerName = channelOwnerName,
                    ChannelOwnerProfileUrl = channelOwnerProfileUrl,
                };
            }
            );

            return response;
        }

        public async Task<IEnumerable<UserManagedChannelListResp>> UserManagedChannelList(UserManagedChannelListReq userManageChannelListReq)
        {
            IEnumerable<Channel> userChannels = await _channelRepo.GetChannelByUserId(userManageChannelListReq.UserId);

            if (userChannels == null)
            {
                throw new BusinessException("Fail to get user managed channels");
            }

            IEnumerable<UserManagedChannelListResp> response = userChannels.Select(channel =>
            {
                return new UserManagedChannelListResp()
                {
                    Topic = channel.Topic,
                    Description = channel.Description,
                    TotalMembers = channel.TotalMember
                };
            });

            return response;
        }
    }
}
