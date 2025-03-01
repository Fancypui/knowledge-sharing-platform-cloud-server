using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowledge_sharing_platform_cloud.Data.Models.Category
{
    public class CategoryContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly string connectionString;

        public CategoryContext(IConfiguration config)
        {
            _config = config;
            connectionString = _config.GetConnectionString("sqlServer");
        }

        public DbSet<Category> Category{ get; set; }

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Category> builder)
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
