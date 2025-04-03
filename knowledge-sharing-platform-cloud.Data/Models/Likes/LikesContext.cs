using knowledge_sharing_platform_cloud.Data.Constant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace knowledge_sharing_platform_cloud.Data.Models.Likes
{
    public class LikesContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly string connectionString;

        public LikesContext(IConfiguration config)
        {
            _config = config;
            connectionString = config.GetConnectionString(ConfigurationConstant.DB_CONNECTION_STRING);
        }
        public DbSet<Likes> Likes{ get; set; }

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Likes> builder)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Likes>()
                .HasIndex(l => new { l.UserId, l.PostId })  // Define unique constraint
                .IsUnique();
        }
    }
}
