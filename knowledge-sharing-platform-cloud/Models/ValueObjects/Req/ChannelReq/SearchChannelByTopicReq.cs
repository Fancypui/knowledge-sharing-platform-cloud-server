namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req.ChannelReq
{
    public class SearchChannelByTopicReq: CursorBaseReq
    {
        public long? UserId { get; set; }
        public string Topic {  get; set; }
    }
}
