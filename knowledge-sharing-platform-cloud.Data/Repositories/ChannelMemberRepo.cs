using knowledge_sharing_platform_cloud.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class ChannelMemberRepo
    {
        private readonly ApplicationContext _applicationContext;

        public ChannelMemberRepo(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<ChannelMember> CreateChanneMemberlAsync(ChannelMember channelMember)
        {
            _applicationContext.ChannelMember.Add(channelMember);
            await _applicationContext.SaveChangesAsync();

            return channelMember;
        }

        public async Task<IEnumerable<long>> GetUserJoinedChannels(long userId)
        {
            return await _applicationContext.ChannelMember
                .Where(c => c.UserId == userId)
                .Select(c => c.ChannelId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChannelMember>> CheckUserJoinChannels(long userId, List<long> channelIds)
        {
            return await _applicationContext.ChannelMember
                .Where(cm => cm.UserId == userId && channelIds.Contains(cm.ChannelId))
                .ToListAsync();
        }

        public async Task<bool> CheckUserJoinChannel(long userId, long channelId)
        {
            return await _applicationContext.ChannelMember
                         .Where(c => c.UserId == userId && c.ChannelId == channelId)
                         .CountAsync() >= 1;
        }

        public async Task<ChannelMember> CreateChannelMemberAsync(ChannelMember channelMember)
        {
            if (channelMember == null) throw new ArgumentNullException(nameof(channelMember));

            _applicationContext.ChannelMember.Add(channelMember);
            _applicationContext.SaveChanges();
            return channelMember;

        }
    }
}
