using knowledge_sharing_platform_cloud.Data.Models.ChannelMember;
using Microsoft.EntityFrameworkCore;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class ChannelMemberRepo
    {
        private readonly ChannelMemberContext _channelMemberContext;

        public ChannelMemberRepo(ChannelMemberContext channelMemberContext)
        {
            _channelMemberContext = channelMemberContext;
        }

        public async Task<ChannelMember> CreateChanneMemberlAsync(ChannelMember channelMember)
        {
            _channelMemberContext.ChannelMember.Add(channelMember);
            await _channelMemberContext.SaveChangesAsync();

            return channelMember;
        }

        public async Task<IEnumerable<long>> GetUserJoinedChannels(long userId)
        {
            return await _channelMemberContext.ChannelMember
                .Where(c => c.UserId == userId)
                .Select(c => c.ChannelId)
                .ToListAsync();
        }

        public async Task<bool> CheckUserJoinChannel(long userId, long channelId)
        {
            return await _channelMemberContext.ChannelMember
                .Where(c => c.UserId == userId && c.ChannelId == channelId)
                .CountAsync() >= 1;
        }
    }
}
