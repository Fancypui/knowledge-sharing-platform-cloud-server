using knowledge_sharing_platform_cloud.Entity;
using Microsoft.EntityFrameworkCore;

namespace knowledge_sharing_platform_cloud.Config
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
        }
    }
}
