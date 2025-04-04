using Azure.Identity;
using knowledge_sharing_platform_cloud.Data.Models;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Data;
using System.Formats.Asn1;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class ChannelRepo
    {
        private readonly ApplicationContext _applicationContext;

        public ChannelRepo(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<Channel> CreateChannelAsync(Channel channel)
        {
            _applicationContext.Channel.Add(channel);
            await _applicationContext.SaveChangesAsync();

            return channel;
        }

        public async Task<Channel> GetChannelbyIdAsync(long id)
        {
            return await _applicationContext.Channel.FindAsync(id);
        }

        public async Task<IEnumerable<Channel>> GetChannelByIds(List<long> ids)
        {
            return await _applicationContext.Channel
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Channel>> GetChannelByUserId(long userId)
        {
            return await _applicationContext.Channel
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> IncreaseTotalMemberByOne(long channelId)
        {
            return await _applicationContext.Channel
                .Where(c => c.Id == channelId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.TotalMember, c => c.TotalMember + 1)) > 0;
        }

        public async Task<IEnumerable<Channel>> GetChannelByName(string topicName, long? skip, int pageSize)
        {
            var channelList = await _applicationContext.Channel
                .Where(c => EF.Functions.Like(c.Topic, $"%{topicName}%"))
                .Skip((int)(skip ?? 0))
                .Take(pageSize)
                .ToListAsync();

            return channelList;
        }

        public async Task<int> TotalMemberByChannelId(long channelId)
        {
            var memberCount = await _applicationContext.Channel
            .Where(c => c.Id == channelId)
            .Select(c => c.TotalMember)
            .FirstOrDefaultAsync();

            return memberCount; 

        }

        public async Task<List<(long Id, int TotalMember)>> GetTop500Channels()
        {
            var result = await _applicationContext.Channel
                .OrderByDescending(c => c.TotalMember)
                .Select(c => new { c.Id, c.TotalMember })
                .Take(500)
                .ToListAsync();
            return result?.Select(c => (c.Id, c.TotalMember)).ToList() ?? new List<(long, int)>();
        }
        public async Task<int> GetChannelCountUpTo500()
        {
            return await _applicationContext.Channel
                .Take(500) 
                .CountAsync(); 
        }


        public async Task<bool> IncreaseTotalPostByOne(long channelId, IDbContextTransaction transaction = null)
        {
            if (transaction != null)
            {
                _applicationContext.Database.UseTransaction(transaction.GetDbTransaction());
            }
            return await _applicationContext.Channel
                .Where(c => c.Id == channelId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.TotalPost, c => c.TotalPost + 1)) > 0;
        }
    }
}
