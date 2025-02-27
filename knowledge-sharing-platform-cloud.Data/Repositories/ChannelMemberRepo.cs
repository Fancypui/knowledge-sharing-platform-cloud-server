using knowledge_sharing_platform_cloud.Data.Models.ChannelMember;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class ChannelMemberRepo
    {
        private readonly ChannelMemberContext _channelMemberContext;

        public ChannelMemberRepo(ChannelMemberContext channelMemberContext)
        {
            _channelMemberContext = channelMemberContext;
        }

        public async Task<ChannelMember> CreateChannelAsync(ChannelMember channelMember)
        {
            _channelMemberContext.ChannelMember.Add(channelMember);
            await _channelMemberContext.SaveChangesAsync();

            return channelMember;
        }
    }
}
