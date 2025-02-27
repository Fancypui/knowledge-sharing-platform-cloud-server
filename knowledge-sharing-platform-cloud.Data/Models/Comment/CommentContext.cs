using knowledge_sharing_platform_cloud.Data.Constant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace knowledge_sharing_platform_cloud.Data.Models.Comment
{
    public class CommentContext:DbContext
    {
        private readonly IConfiguration _config;
        private readonly string connectionString;

        public CommentContext(IConfiguration config)
        {
            _config = config;
            connectionString = config.GetConnectionString(ConfigurationConstant.DB_CONNECTION_STRING);
        }
        public DbSet<Comment> Comment { get; set; }

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Comment> builder)
        {
           
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
