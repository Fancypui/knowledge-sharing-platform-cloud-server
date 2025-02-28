using knowledge_sharing_platform_cloud.Data.Models;
using Stripe;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface IStripeService
    {
        Account CreateStripeAccount(User user);
        PaymentLink CreateStripePaymentLink(string stripeAccountId, string stripePriceId, long subscriberUserId, long channelId);
        Price CreateStripeProductPrice(string stripeAccountId, decimal productPrice, string productName);
    }
}