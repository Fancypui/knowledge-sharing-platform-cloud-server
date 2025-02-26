using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace knowledge_sharing_platform_cloud.Data.Models;

public class UserContext : DbContext
{

    private readonly IConfiguration _config;
    private readonly string connectionString;

    public UserContext(IConfiguration config)
    {
        _config = config;
        connectionString = config.GetConnectionString("default");
    }

    public DbSet<User> User { get; set; }

    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(connectionString);
    }
}
