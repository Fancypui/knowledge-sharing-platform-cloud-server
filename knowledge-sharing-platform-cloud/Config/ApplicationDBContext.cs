using knowledge_sharing_platform_cloud.Entity;
using System.Data;
using System.Reflection;
using System;
using Microsoft.EntityFrameworkCore;

namespace knowledge_sharing_platform_cloud.config
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        //public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
