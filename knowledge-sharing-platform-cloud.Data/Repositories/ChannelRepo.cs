using knowledge_sharing_platform_cloud.Data.Models.Channel;
using System.Formats.Asn1;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class ChannelRepo
    {
        private readonly ChannelContext _channelContext;

        public ChannelRepo(ChannelContext channelContext) { 
            _channelContext = channelContext;
        }

        public async Task<Channel> CreateChannelAsync(Channel channel)
        {
            _channelContext.Channel.Add(channel);
            await _channelContext.SaveChangesAsync();

            return channel;
        }

        public async Task<Channel> GetChannelbyIdAsync(long id)
        {
            return await _channelContext.Channel.FindAsync(id);
        }
    }
}
