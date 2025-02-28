using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Models.Channel;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using Stripe;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class ChannelServiceImpl : IChannelService
    {
        private readonly ChannelRepo _channelRepo;
        private readonly UserRepo _userRepo;
        private readonly IConfiguration _configuration;

        public ChannelServiceImpl(ChannelRepo channelRepo, UserRepo userRepo, IConfiguration configuration)
        {
            _channelRepo = channelRepo;
            _userRepo = userRepo;
            _configuration = configuration;

            StripeConfiguration.ApiKey = _configuration["StripeApiSecretKey"];
        }

        public async Task<CreateChannelResp> CreateChannel(CreateChannelReq createChannelReq)
        {
            User user = await _userRepo.GetUserByIdAsync(createChannelReq.userId);

            // create new Stripe Account for users who create a channel for the first time
            if (user.StripeAccountId == null) {
                var accountOptions = new AccountCreateOptions
                {
                    Country = "MY",
                    Email = user.Email,
                    Capabilities = new AccountCapabilitiesOptions
                    {
                        CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                        Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                        LinkPayments = new AccountCapabilitiesLinkPaymentsOptions { Requested = true },
                    }
                };
                var accountService = new AccountService();
                var newAccount = accountService.Create(accountOptions);

                user.StripeAccountId = newAccount.Id;

                await _userRepo.UpdateUserAsync(user);
            };

            // connect to the user Stripe account
            var connectAccountOption = new RequestOptions
            {
                StripeAccount = user.StripeAccountId,
            };

            // create the channel as a product in the user's Stripe account
            var priceOption = new PriceCreateOptions
            {
                Currency = "myr",
                UnitAmount = (long?)createChannelReq.subscriptionFee * 100,
                ProductData = new PriceProductDataOptions { Name = createChannelReq.topic },
            };

            var priceService = new PriceService();
            var newPrice = priceService.Create(priceOption, connectAccountOption);

            // create new record of channel in db
            Channel newChannel = new()
            {
                Topic = createChannelReq.topic,
                Description = createChannelReq.description,
                UserId = createChannelReq.userId,
                ChannelImgUrl = createChannelReq.channelImgUrl,
                ChannelImgBackground = createChannelReq.channelImgBackground,
                SubscriptionFee = createChannelReq.subscriptionFee,
                StripePriceId = newPrice.Id,
            };

            await _channelRepo.CreateChannelAsync(newChannel);

            CreateChannelResp response = new()
            {
                ChannelId = newChannel.Id,
            };

            return response; 
        }

        public async Task<ApiResult<JoinChannelResp>> JoinChannel(JoinChannelReq joinChannelReq)
        {
            Channel channelToBeJoined = await _channelRepo.GetChannelbyIdAsync(joinChannelReq.channelId);

            if (channelToBeJoined == null)
            {
                return ApiResult<JoinChannelResp>.ServiceFail(1, "Channel does not exist");
            }

            User channelCreator = await _userRepo.GetUserByIdAsync(channelToBeJoined.UserId);

            if (channelCreator == null)
            {
                return ApiResult<JoinChannelResp>.ServiceFail(1, "Channel creator does not exist");
            }

            var accountOptions = new RequestOptions
            {
                StripeAccount = channelCreator.StripeAccountId,
            };

            var paymentLinkOptions = new PaymentLinkCreateOptions
            {
                LineItems = new List<PaymentLinkLineItemOptions>
                {
                    new PaymentLinkLineItemOptions {
                        Price = channelToBeJoined.StripePriceId,
                        Quantity = 1,
                    },
                },
                Metadata = new Dictionary<string, string>
                {
                    { "userId", joinChannelReq.userId.ToString() }
                }
            };

            var paymentLinkService = new PaymentLinkService();
            var paymentLink = paymentLinkService.Create(paymentLinkOptions, accountOptions);

            JoinChannelResp response = new()
            {
                paymentLinkUrl = paymentLink.Url
            };

            return ApiResult<JoinChannelResp>.ServiceSucess(response);
        }
    }
}
