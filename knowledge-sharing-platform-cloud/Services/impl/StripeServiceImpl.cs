using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using Stripe;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class StripeServiceImpl : IStripeService
    {
        public Account CreateStripeAccount(User user)
        {
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
            Account newAccount = accountService.Create(accountOptions);

            if (newAccount == null)
            {
                throw new BusinessException((int)CommonErrorEnum.BUSINESS_ERROR, "Fail to create stripe account");
            }

            return newAccount;
        }

        private RequestOptions ConnectUserStripeAccount(string stripeAccountId)
        {
            // connect to the user Stripe account
            var connectAccountOption = new RequestOptions
            {
                StripeAccount = stripeAccountId,
            };

            return connectAccountOption;
        }

        public Price CreateStripeProductPrice(string stripeAccountId, decimal productPrice, string productName)
        {
            RequestOptions connectAccountOption = ConnectUserStripeAccount(stripeAccountId);

            var priceOption = new PriceCreateOptions
            {
                Currency = "myr",
                UnitAmount = (long?)productPrice * 100,
                ProductData = new PriceProductDataOptions { Name = productName },
            };

            var priceService = new PriceService();
            var newPrice = priceService.Create(priceOption, connectAccountOption);

            if (newPrice == null)
            {
                throw new BusinessException((int)CommonErrorEnum.BUSINESS_ERROR, "Fail to create stripe price");
            }

            return newPrice;
        }

        public PaymentLink CreateStripePaymentLink(string stripeAccountId, string stripePriceId, long subscriberUserId, long channelId)
        {
            RequestOptions connectAccountOption = ConnectUserStripeAccount(stripeAccountId);

            var paymentLinkOptions = new PaymentLinkCreateOptions
            {
                LineItems = new List<PaymentLinkLineItemOptions>
                {
                    new PaymentLinkLineItemOptions {
                        Price = stripePriceId,
                        Quantity = 1,
                    },
                },
                Metadata = new Dictionary<string, string>
                {
                    { "userId", subscriberUserId.ToString() },
                    { "channelId", channelId.ToString() }
                }
            };

            var paymentLinkService = new PaymentLinkService();
            var paymentLink = paymentLinkService.Create(paymentLinkOptions, connectAccountOption);

            if (paymentLink == null)
            {
                throw new BusinessException((int)CommonErrorEnum.BUSINESS_ERROR, "Fail to create stripe paymment link");
            }

            return paymentLink;
        }
    }
}
