using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Models.Channel;
using knowledge_sharing_platform_cloud.Data.Models.ChannelMember;
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
        private readonly ChannelSummaryCache _channelSummaryCache;
        private readonly UserCache _userCache;
        private readonly ChannelLeaderboardCache _leaderboardCache;

        private readonly IConfiguration _configuration;
        public ChannelServiceImpl(
            ChannelRepo channelRepo,
            UserRepo userRepo,
            ChannelMemberRepo channelMemberRepo,
            ChannelSummaryCache channelSummaryCache,
            UserCache userCache,
            ChannelLeaderboardCache leaderboardCache,
            IStripeService stripeService,
            IConfiguration configuration)
        {
            _channelRepo = channelRepo;
            _userRepo = userRepo;
            _channelMemberRepo = channelMemberRepo;
            _stripeService = stripeService;
            _channelSummaryCache = channelSummaryCache;
            _userCache = userCache;
            _leaderboardCache = leaderboardCache;
            _configuration = configuration;
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
            Channel newChannel = new()
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

            CreateChannelResp response = new()
            {
                ChannelId = newChannel.Id,
            };

            return response; 
        }

        public async Task<JoinChannelResp> JoinChannel(JoinChannelReq joinChannelReq)
        {
            Channel channelToBeJoined = await _channelRepo.GetChannelbyIdAsync(joinChannelReq.ChannelId);

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

        public async Task<String> JoinChannelSuccess(string userId, string channelId, decimal feePaid)
        {
            ChannelMember channelMember = new()
            {
                UserId = long.Parse(userId),
                ChannelId = long.Parse(channelId),
                SubscriptionFeePaid = feePaid,
            };

            await _channelMemberRepo.CreateChanneMemberlAsync(channelMember);

            await _channelRepo.IncreaseTotalMemberByOne(long.Parse(channelId));

            // push 

            return "nice";
        }

        public async Task<String> JoinChannelFail(string userId, string channelId, decimal feePaid)
        {
            ChannelMember channelMember = new()
            {
                UserId = long.Parse(userId),
                ChannelId = long.Parse(channelId),
                SubscriptionFeePaid = feePaid,
            };

            await _channelMemberRepo.CreateChanneMemberlAsync(channelMember);

            // use websocket to emit message to FE on payment success

            return "nice";
        }

        public async Task<GetChannelSummaryResp> GetChannelSummary(GetChannelSummaryReq getChannelSummaryReq)
        {
            Channel channel = await _channelRepo.GetChannelbyIdAsync(getChannelSummaryReq.ChannelId);

            if (channel == null)
            {
                throw new BusinessException((int)CommonErrorEnum.BUSINESS_ERROR, "Fail to get channel summary. Channel does not exist in db");
            }

            TimeSpan channelOperationDuration = DateTime.Now - channel.CreatedTime;

            bool isUserJoinedChannel = await _channelMemberRepo.CheckUserJoinChannel(getChannelSummaryReq.UserId, getChannelSummaryReq.ChannelId);

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
            Channel channel = await _channelRepo.GetChannelbyIdAsync(getChannelOwnerSummaryReq.ChannelId);

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

        public async Task<IEnumerable<SearchChannelByTopicResp>> SearchChannelByTopic(SearchChannelByTopicReq searchChannelByTopicReq)
        {
            IEnumerable<Channel> channelList = await _channelRepo.GetChannelByName(searchChannelByTopicReq.Topic);

            IEnumerable<SearchChannelByTopicResp> response = channelList.Select(channel =>
            {
                return new SearchChannelByTopicResp()
                {
                    ChannelId = channel.Id,
                    ChannelTopic = channel.Topic,
                };
            });

            return response;
        }

        public async Task<CursorBasedResp<IEnumerable<ChannelLeaderboardListResp>>> ChannelLeaderboardList(CursorBaseReq request)
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
                return CursorBasedResp<IEnumerable<ChannelLeaderboardListResp>>.empty();
            }
            /**
             * convert the entries into list of channel id
             */
            var channelIds = channelLeaderboardEntries.Select(c => c.Id).ToList();
            var channelSummary = await _channelSummaryCache.GetBatch(channelIds);
            var listData = channelLeaderboardEntries.Select(entry =>
            {
                var channel = channelSummary.GetValueOrDefault(entry.Id, null);
                return new ChannelLeaderboardListResp
                {
                    ChannelTitle = channel?.Topic ?? null,
                    ChannelId = entry.Id,
                    TotalMemberCount = entry.TotalMember,
                    ChannelDescription = channel?.Topic ?? null,
                    ChannelProfileUrl = channel?.ChannelImgUrl ?? null,
                    ChannelBackgroundUrl = channel?.ChannelImgUrl ?? null,

                };

            });
            if (cursor == null)
            {
                cursor = request.PageSize;
            }
            else
            {
                cursor = cursor + listData.Count();
            }
            //long? nextCursor = listData.Any() ? listData.Max(x => x.ChannelId) : null;
            return CursorBasedResp<IEnumerable<ChannelLeaderboardListResp>>.Init(new List<IEnumerable<ChannelLeaderboardListResp>> { listData }, cursor, listData.Count() < request.PageSize);


        }
    }
}
