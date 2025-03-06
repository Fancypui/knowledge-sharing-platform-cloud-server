using System.Data.Common;
using knowledge_sharing_platform_cloud.Data.Constant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace knowledge_sharing_platform_cloud.Data.Models.Channel
{
    public class ChannelContext : DbContext
    {

        private readonly DbConnection _connection;

        public ChannelContext(DbConnection connection)
        {
            _connection = connection;
        }

        public DbSet<Channel> Channel { get; set; }

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Channel> builder)
        {
            builder.HasKey(u => u.Id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connection != null)
            {
                // Use the shared connection
                optionsBuilder.UseSqlServer(_connection);
            }
            else
            {
                // Fallback to creating a new connection (for backward compatibility)
                base.OnConfiguring(optionsBuilder);
            }
        }
    }
}
