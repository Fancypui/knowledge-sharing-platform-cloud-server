using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using knowledge_sharing_platform_cloud.Data.Constant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace knowledge_sharing_platform_cloud.Data.Models.Post
{
    public class PostContext : DbContext
    {

        private readonly DbConnection _connection;

        public PostContext(DbConnection connection)
        {
            _connection = connection;
        }
        public DbSet<Post> Post { get; set; }

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Post> builder)
        {

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
