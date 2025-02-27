using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace knowledge_sharing_platform_cloud.Data.Models.ChannelMember
{
    public class ChannelMemberContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly string connectionString;

        public ChannelMemberContext(IConfiguration config)
        {
            _config = config;
            connectionString = _config.GetConnectionString("sqlServer");
        }

        public DbSet<ChannelMember> ChannelMember { get; set; }

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ChannelMember> builder)
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
