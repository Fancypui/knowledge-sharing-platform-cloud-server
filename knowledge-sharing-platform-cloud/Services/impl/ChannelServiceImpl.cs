using System.Text.Json;
using System.Threading.Channels;
using AWS.Messaging;
using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.DTO;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.ChannelReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.ChannelResp;
using Stripe;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class ChannelServiceImpl : IChannelService
    {
        private readonly ChannelRepo _channelRepo;
        private readonly UserRepo _userRepo;
        private readonly ChannelMemberRepo _channelMemberRepo;
        private readonly IStripeService _stripeService;
        private readonly S3Service _s3Service;
        private readonly ChannelSummaryCache _channelSummaryCache;
        private readonly UserCache _userCache;
        private readonly ChannelLeaderboardCache _leaderboardCache;
        private readonly IMessagePublisher _messagePublisher;

        private readonly IConfiguration _configuration;
        public ChannelServiceImpl(
            ChannelRepo channelRepo,
            UserRepo userRepo,
            ChannelMemberRepo channelMemberRepo,
            ChannelSummaryCache channelSummaryCache,
            UserCache userCache,
            ChannelLeaderboardCache leaderboardCache,
            S3Service s3Service,
            IStripeService stripeService,
            IMessagePublisher messagePublisher,
            IConfiguration configuration)
        {
            _channelRepo = channelRepo;
            _userRepo = userRepo;
            _channelMemberRepo = channelMemberRepo;
            _s3Service = s3Service;
            _stripeService = stripeService;
            _channelSummaryCache = channelSummaryCache;
            _userCache = userCache;
            _leaderboardCache = leaderboardCache;
            _configuration = configuration;
            _messagePublisher = messagePublisher;
        }

        public async Task<CreateChannelResp> CreateChannel(CreateChannelReq createChannelReq)
        {
            User user = await _userRepo.GetUserByIdAsync(createChannelReq.UserId);

            if (user == null)
            {
                throw new BusinessException((int)CommonErrorEnum.BUSINESS_ERROR, "Fail to create channel. User does not exist in db");
            }

            // create new Stripe Account for users who create a channel for the first time
            if (user.StripeAccountId == null) {
                Account newAccount = _stripeService.CreateStripeAccount(user);

                user.StripeAccountId = newAccount.Id;

                await _userRepo.UpdateUserAsync(user);
            };

            // create the channel as a product in the user's Stripe account
            string userStripeAccountId = user.StripeAccountId;
            decimal productPrice = createChannelReq.SubscriptionFee;
            string productName = createChannelReq.Topic;
            Price channelAsStripeProduct = _stripeService.CreateStripeProductPrice(userStripeAccountId, productPrice, productName);

            // create new record of channel in db
            Data.Models.Channel newChannel = new()
            {
                Topic = createChannelReq.Topic,
                Description = createChannelReq.Description,
                UserId = createChannelReq.UserId,
                ChannelImgUrl = createChannelReq.ChannelImgUrl,
                ChannelImgBackground = createChannelReq.ChannelImgBackground,
                SubscriptionFee = createChannelReq.SubscriptionFee,
                StripePriceId = channelAsStripeProduct.Id,
            };

            await _channelRepo.CreateChannelAsync(newChannel);

            try
            {
                var channelLeaderboardDTO = new ChannelLeaderboardDTO
                {
                    channelId = newChannel.Id
                };
                await _messagePublisher.PublishAsync(channelLeaderboardDTO);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }

            CreateChannelResp response = new()
            {
                ChannelId = newChannel.Id,
            };

            return response; 
        }

        public async Task<JoinChannelResp> JoinChannel(JoinChannelReq joinChannelReq)
        {
            var channelToBeJoined = await _channelRepo.GetChannelbyIdAsync(joinChannelReq.ChannelId);

            if (channelToBeJoined == null)
            {
                throw new BusinessException((int)CommonErrorEnum.BUSINESS_ERROR, "Fail to join channel. Channel does not exist in db");
            }

            User channelCreator = await _userRepo.GetUserByIdAsync(channelToBeJoined.UserId);

            if (channelCreator == null)
            {
                throw new BusinessException((int)CommonErrorEnum.BUSINESS_ERROR, "Fail to join channel. Channel creator does not exist in db");
            }

            if (channelCreator.StripeAccountId == null)
            {
                throw new BusinessException((int)CommonErrorEnum.BUSINESS_ERROR, "Fail to join channel. Channel creator does not have a Stripe account");
            }

            // generate a Stripe payment link for the user to join a specific channel
            string channelCreatorStripeAccountId = channelCreator.StripeAccountId;
            string channelStripePriceId = channelToBeJoined.StripePriceId;
            long subscriberUserId = joinChannelReq.UserId;
            long channelId = joinChannelReq.ChannelId;
            PaymentLink paymentLink = _stripeService.CreateStripePaymentLink(channelCreatorStripeAccountId, channelStripePriceId, subscriberUserId, channelId);

            JoinChannelResp response = new()
            {
                PaymentLinkUrl = paymentLink.Url
            };

            return response;
        }

        public async Task<JoinChannelSuccessResp> JoinChannelSuccess(string userId, string channelId, decimal feePaid)
        {
            ChannelMember channelMember = new()
            {
                UserId = long.Parse(userId),
                ChannelId = long.Parse(channelId),
                SubscriptionFeePaid = feePaid / 100,
            };

            await _channelMemberRepo.CreateChannelMemberAsync(channelMember);

            await _channelRepo.IncreaseTotalMemberByOne(long.Parse(channelId));
            

            JoinChannelSuccessResp response = new()
            {
                ChannelId = long.Parse(channelId),
            };

            WSRespBase<JoinChannelSuccessResp> wsResponse = new()
            {
                Type = (int)Enum.WSRespTypeEnum.PAYMENT_SUCCESS,
                Data = response,
            };

            PushNotificationDTO pushPaymentSuccessNotification = new()
            {
                UserIdList = [long.Parse(userId)],
                Type = Enum.PushNotificationType.SEND_TO_INDIVIDUAL,
            };

            pushPaymentSuccessNotification.SetResp(wsResponse);

            try
            {
                var channelLeaderboardDTO = new ChannelLeaderboardDTO
                {
                    channelId = long.Parse(channelId)
                };
                
                await _messagePublisher.PublishAsync(pushPaymentSuccessNotification);
                await _messagePublisher.PublishAsync(channelLeaderboardDTO);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }

            return response;
        }

        public async Task<JoinChannelFailResp> JoinChannelFail(string channelId)
        {
            // use websocket to emit message to FE on payment success

            JoinChannelFailResp response = new()
            {
                ChannelId = long.Parse(channelId),
            };

            WSRespBase<JoinChannelFailResp> wsResponse = new()
            {
                Type = (int)Enum.WSRespTypeEnum.PAYMENT_FAIL,
                Data = response,
            };

            PushNotificationDTO pushPaymentFailNotification = new()
            {
                RespJson = JsonSerializer.Serialize(wsResponse),
                UserIdList = [],
                Type = Enum.PushNotificationType.SEND_TO_INDIVIDUAL,
            };

            await _messagePublisher.PublishAsync(pushPaymentFailNotification);

            return response;
        }

        public async Task<GetChannelSummaryResp> GetChannelSummary(GetChannelSummaryReq getChannelSummaryReq, long uid)
        {
            var channel = await _channelRepo.GetChannelbyIdAsync(getChannelSummaryReq.ChannelId);

            if (channel == null)
            {
                throw new BusinessException((int)CommonErrorEnum.BUSINESS_ERROR, "Fail to get channel summary. Channel does not exist in db");
            }

            TimeSpan channelOperationDuration = DateTime.Now - channel.CreatedTime;

            bool isUserJoinedChannel = await _channelMemberRepo.CheckUserJoinChannel(uid, getChannelSummaryReq.ChannelId);

            GetChannelSummaryResp response = new()
            {
                ChannelId = channel.Id,
                Topic = channel.Topic,
                Description = channel.Description,
                ChannelImgUrl = channel.ChannelImgUrl,
                ChannelImgBackground = channel.ChannelImgBackground,
                TotalMember = channel.TotalMember,
                TotalPost  = channel.TotalPost,
                OperationDuration = channelOperationDuration.TotalDays,
                SubscriptionFee = isUserJoinedChannel ? channel.SubscriptionFee : null
            };

            return response;    
        }

        public async Task<GetChannelOwnerSummaryResp> GetChannelOwnerSummary(GetChannelOwnerSummaryReq getChannelOwnerSummaryReq)
        {
            var channel = await _channelRepo.GetChannelbyIdAsync(getChannelOwnerSummaryReq.ChannelId);

            if (channel == null)
            {
                throw new BusinessException((int)CommonErrorEnum.BUSINESS_ERROR, "Fail to get channel owner summary. Channel does not exist in db");
            }

            User channelOwner = await _userRepo.GetUserByIdAsync(channel.UserId);

            if (channelOwner == null)
            {
                throw new BusinessException((int)CommonErrorEnum.BUSINESS_ERROR, "Fail to get channel owner summary. Channel owner account does not exist in db");
            }

            GetChannelOwnerSummaryResp response = new()
            {
                ChannelId = channel.Id,
                UserId = channel.UserId,
                Username = channelOwner.Username,
                UserDescription = channelOwner.Description,
                UserProfileUrl = channelOwner.ProfileUrl
            };

            return response;
        }

        public async Task<CursorBasedResp<SearchChannelByTopicResp>> SearchChannelByTopic(SearchChannelByTopicReq searchChannelByTopicReq)
        {
            
            /**
             * cursor conversion
             */
            long? cursor = null;
            if (!searchChannelByTopicReq.IsFirstPage() && long.TryParse(searchChannelByTopicReq.Cursor, out var parsedCursor))
            {
                cursor = parsedCursor;
            }

            var channelList = await _channelRepo.GetChannelByName(searchChannelByTopicReq.Topic, cursor, searchChannelByTopicReq.PageSize);

            var parentIds = channelList.Select(c => c.UserId).Where(c => c != 0).Distinct().ToList();

            Dictionary<long, User> userMap = new Dictionary<long, User>();

            if (parentIds.Any())
            {
                IEnumerable<User> result = await _userRepo.UserListByIds(parentIds);

                userMap = result.ToDictionary(user => user.Id, user => user);
            }


            IEnumerable<SearchChannelByTopicResp> listData = await Task.WhenAll(channelList.Select(async channel =>
            {
                //bool isUserJoinedChannel = await _channelMemberRepo.CheckUserJoinChannel(searchChannelByTopicReq.UserId, channel.Id);

                string? channelImgBackgroundPresignedUrl = null;
                if (!string.IsNullOrWhiteSpace(channel.ChannelImgBackground))
                {
                    GetS3PresignedUrlReq s3Request = new()
                    {
                        ObjectKey = channel.ChannelImgBackground
                    };

                    GetS3PresignedUrlResp s3Response = await _s3Service.GeneratePresignedUrlToRetrieve([s3Request]);

                    channelImgBackgroundPresignedUrl = s3Response.S3PresignedUrls.FirstOrDefault();
                }


                User? channelOwner = userMap.GetValueOrDefault(channel.UserId, null);



                return new SearchChannelByTopicResp()
                {
                    ChannelId = channel.Id,
                    ChannelTopic = channel.Topic,
                    SubscriptionFee = channel.SubscriptionFee,
                    ChannelDesc = channel.Description,
                    ChannelImgUrl = channelOwner?.Username ?? string.Empty,
                    ChannelImgBackground = channelImgBackgroundPresignedUrl??"",
                    //IsUserJoined = isUserJoinedChannel,
                };
            }));

            long? cursorId = listData.Any() ? listData.Min(channelSearchResult => channelSearchResult.ChannelId) : null;

            return CursorBasedResp<SearchChannelByTopicResp>.Init(listData, cursorId, listData.Count() < searchChannelByTopicReq.PageSize);
        }

        public async Task<CursorBasedResp<ChannelLeaderboardListResp>> ChannelLeaderboardList(CursorBaseReq request)
        {
            long? cursor = null;
            if (!request.IsFirstPage() && long.TryParse(request.Cursor, out var parsedCursor))
            {
                cursor = parsedCursor;
            }
            /**
             * get leaderboard from cache
             */
            var channelLeaderboardEntries = await _leaderboardCache.GetLeaderboardPage(cursor, request.PageSize);
            // If channelIds is empty, return an empty list immediately
            if (channelLeaderboardEntries == null || !channelLeaderboardEntries.Any())
            {
                return CursorBasedResp<ChannelLeaderboardListResp>.empty();
            }
            /**
             * convert the entries into list of channel id
             */
            var channelIds = channelLeaderboardEntries.Select(c => c.Id).ToList();
            var channelSummary = await _channelSummaryCache.GetBatch(channelIds);
            var listData = await Task.WhenAll(channelLeaderboardEntries.Select(async entry =>
            {
                var channel = channelSummary.GetValueOrDefault(entry.Id, null);

                string? channelBackgroundPresignedUrl = null;


                if (!string.IsNullOrWhiteSpace(channel?.ChannelImgBackground))
                {
                    GetS3PresignedUrlReq s3Request = new()
                    {
                        ObjectKey = channel.ChannelImgBackground
                    };

                    var s3Response = await _s3Service.GeneratePresignedUrlToRetrieve([s3Request]);

                    channelBackgroundPresignedUrl = s3Response?.S3PresignedUrls?.FirstOrDefault();
                }


                return new ChannelLeaderboardListResp
                {
                    ChannelTitle = channel?.Topic ?? null,
                    ChannelId = entry.Id,
                    TotalMemberCount = entry.TotalMember,
                    ChannelDescription = channel?.Description ?? null,
                    ChannelProfileUrl = channel?.ChannelImgUrl ?? null,
                    ChannelBackgroundUrl = channelBackgroundPresignedUrl,

                };

            }).ToList());
            if (cursor == null)
            {
                cursor = listData.Count();
            }
            else
            {
                cursor = cursor + listData.Count();
            }
            //long? nextCursor = listData.Any() ? listData.Max(x => x.ChannelId) : null;
            return CursorBasedResp<ChannelLeaderboardListResp>.Init(listData, cursor, listData.Count() < request.PageSize);


        }
    }
}
