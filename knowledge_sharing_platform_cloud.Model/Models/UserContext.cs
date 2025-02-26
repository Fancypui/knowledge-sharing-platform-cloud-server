using Microsoft.EntityFrameworkCore;

namespace knowledge_sharing_platform_cloud.Data.Models;

public class UserContext : DbContext
{
    public DbSet<User> User { get; set; }

    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
    }
}
