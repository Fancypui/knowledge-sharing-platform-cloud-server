using Microsoft.AspNetCore.Routing.Constraints;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp
{
    public class ChannelLeaderboardListResp
    {
        public string ChannelTitle { get; set; }
        public string ChannelDescription { get; set; }
        public long ChannelId { get; set; }
        public string ChannelProfileUrl { get; set; }
        public string ChannelBackgroundUrl { get; set; }
        public int TotalMemberCount { get; set; }
    }
}
