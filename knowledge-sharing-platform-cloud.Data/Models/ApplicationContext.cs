using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using knowledge_sharing_platform_cloud.Data.Constant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace knowledge_sharing_platform_cloud.Data.Models
{
    public class ApplicationContext:DbContext
    {
        private readonly IConfiguration _config;
        private readonly string connectionString;

        public ApplicationContext(IConfiguration config)
        {
            _config = config;
            connectionString = _config.GetConnectionString(ConfigurationConstant.DB_CONNECTION_STRING);
        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Channel> Channel { get; set; }
        public DbSet<ChannelMember> ChannelMember { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /**
             * user table definition
             */
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email) // Create Unique Index for Email
                .IsUnique()
                .HasDatabaseName("UQ_User_Email");

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .UseIdentityColumn(11000, 1); // Identity start from 11000

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedTime)
                .HasDefaultValueSql("GETDATE()"); // Default timestamp

            /**
             * channel table definition
             */
            modelBuilder.Entity<Channel>()
                .Property(c => c.Id)
                .UseIdentityColumn(21000, 1);

            modelBuilder.Entity<Channel>()
            .Property(c => c.CreatedTime)
            .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Channel>()
                .HasOne<User>()          // Target entity
                .WithMany()              // User can have many Channels
                .HasForeignKey(c => c.UserId)  // FK property in Channel
                .IsRequired()            // NOT NULL constraint
                .OnDelete(DeleteBehavior.Cascade); // Or Cascade/SetNull

            /**
             * Channel Category Table Definition
             */
            modelBuilder.Entity<Category>()
                .Property(c => c.Id)
                .UseIdentityColumn(1, 1);
            modelBuilder.Entity<Category>()
                .HasKey(cc => cc.Id);
            modelBuilder.Entity<Category>()
                .HasOne<Channel>()
                .WithMany()  
                .HasForeignKey(cc => cc.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);  // If Channel is deleted, delete corresponding ChannelCategory

            /**
             * ChannelMember table definition
             */
            modelBuilder.Entity<ChannelMember>()
                .Property(c => c.Id)
                .UseIdentityColumn(1, 1);
            modelBuilder.Entity<ChannelMember>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<ChannelMember>()
                .HasOne<User>()            
                .WithMany()                       
                .HasForeignKey(cm => cm.UserId)   
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction); // Change to No Action to prevent cycle 

            modelBuilder.Entity<ChannelMember>()
                .HasOne<Channel>()     
                .WithMany()                      
                .HasForeignKey(cm => cm.ChannelId) 
                .IsRequired()                     
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChannelMember>()
               .HasIndex(c => c.CheckoutSessionId) // Create Unique Index for Checkout Session Id
               .IsUnique()
               .HasDatabaseName("CM_Checkout_Session_Id");
            /**
             * Post table definition
             */
            modelBuilder.Entity<Post>()
                .Property(p => p.Id)
                .UseIdentityColumn(51000, 1);
            modelBuilder.Entity<Post>()
                .Property(p => p.CreatedTime)
                .HasDefaultValueSql("GETDATE()"); // Default timestamp

            modelBuilder.Entity<Post>()
                .Property(p => p.UpdatedTime)
                .HasDefaultValueSql("GETDATE()"); // Default timestamp
            modelBuilder.Entity<Post>()
                .HasOne<Category>()          
                .WithMany()                 
                .HasForeignKey(p => p.CategoryId)  
                .IsRequired()               
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Post>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            /*
             * comment table definition
             */
            modelBuilder.Entity<Comment>()
                .Property(c => c.Id)
                .UseIdentityColumn(1, 1);
            modelBuilder.Entity<Comment>()
            .Property(c => c.CreatedTime)
            .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Comment>()
                .HasOne<Post>()          // Target entity (Post)
                .WithMany()              // One Post can have many Comments
                .HasForeignKey(c => c.PostId)  // FK property in Comment
                .IsRequired()            // NOT NULL constraint
                .OnDelete(DeleteBehavior.NoAction);

            /**
             * likes table definition
             */
            modelBuilder.Entity<Likes>()
                .Property(l => l.Id)
                .UseIdentityColumn(61000, 1); // Identity starts at 61000

            modelBuilder.Entity<Likes>()
                .Property(l => l.LikeStatus)
                .HasDefaultValue(true);

            
            modelBuilder.Entity<Likes>()
                .HasOne<Post>()         
                .WithMany()             
                .HasForeignKey(l => l.PostId)
                .IsRequired()          
                .OnDelete(DeleteBehavior.Cascade);

          
            modelBuilder.Entity<Likes>()
                .HasOne<User>()         
                .WithMany()             
                .HasForeignKey(l => l.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction); // Change to SetNull


            modelBuilder.Entity<Likes>()
                .HasIndex(l => new { l.UserId, l.PostId })
                .IsUnique()
                .HasDatabaseName("UQ_Likes_UserId_PostId");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(connectionString);
        }

    }
}
