using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace knowledge_sharing_platform_cloud.Data.Models.Channel
{
    public class ChannelContext : DbContext
    {

        private readonly IConfiguration _config;
        private readonly string connectionString;

        public ChannelContext(IConfiguration config)
        {
            _config = config;
            connectionString = _config.GetConnectionString("sqlServer");
        }

        public DbSet<Channel> Channel { get; set; }

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Channel> builder)
        {
            builder.HasKey(u => u.Id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
