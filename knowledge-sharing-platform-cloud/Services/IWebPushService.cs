namespace knowledge_sharing_platform_cloud.Services
{
    public interface IWebPushService
    {
        Task PushChannelPaymentMsgToClientWeb(long userId, long channelId, string pushMsg,
            string redirectUrlWithoutFrontendDomainName);
    }
}
