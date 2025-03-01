using knowledge_sharing_platform_cloud.Data.Constant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace knowledge_sharing_platform_cloud.Data.Models.Post
{
    public class PostContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly string connectionString;

        public PostContext(IConfiguration config)
        {
            _config = config;
            connectionString = config.GetConnectionString(ConfigurationConstant.DB_CONNECTION_STRING);
        }
        public DbSet<Post> Post { get; set; }

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Post> builder)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
