using System.Runtime.CompilerServices;
using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Services.impl;
using knowledge_sharing_platform_cloud.Services;
using StackExchange.Redis;
using knowledge_sharing_platform_cloud.Data.Models.Comment;
using knowledge_sharing_platform_cloud.Data.Models.Post;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Data.Models.Channel;
using knowledge_sharing_platform_cloud.Data.Models.Category;
using knowledge_sharing_platform_cloud.Data.Models.Likes;
using knowledge_sharing_platform_cloud.Data.Models.ChannelMember;
using knowledge_sharing_platform_cloud.Websocket;

namespace knowledge_sharing_platform_cloud.Config
{
    public static class ServiceConfig
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            /**
             * redis dependency injection
             */
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConfig = configuration.GetConnectionString("redis");
                return ConnectionMultiplexer.Connect(redisConfig);
            });
            /**
             * global exception handler configuration
             */
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            /**
             * cache dependency injection
             */
            services.AddScoped<CommentCache, CommentCache>();
            //services.AddScoped<PostImgUrlsCache, PostImgUrlsCache>();
            services.AddScoped<UserCache, UserCache>();
            services.AddScoped<ChannelSummaryCache, ChannelSummaryCache>();
            services.AddScoped<ChannelLeaderboardCache, ChannelLeaderboardCache>();
            /**
             * add controllers, swagger, endpoint config
             */
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            /**
             * repo and context dependency injection
             */
            services.AddScoped<UserRepo, UserRepo>();
            services.AddScoped<CommentRepo, CommentRepo>();
            services.AddScoped<PostRepo, PostRepo>();
            services.AddScoped<PostContext, PostContext>();
            services.AddScoped<UserContext, UserContext>();
            services.AddScoped<CommentContext, CommentContext>();
            services.AddScoped<ChannelRepo>();
            services.AddScoped<ChannelContext>();
            services.AddScoped<CategoryRepo>();
            services.AddScoped<CategoryContext>();
            services.AddTransient<LikesRepo>();
            services.AddTransient<LikesContext>();
            services.AddTransient<ChannelMemberRepo>();
            services.AddTransient<ChannelMemberContext>();

            /**
             * services dependency injection
             */
            services.AddScoped<ICommentSerivce, CommentServiceImpl>();
            services.AddSingleton<IWebsocketService, WebsocketServiceImpl>();
            services.AddScoped<IUserService, UserServiceImpl>();
            services.AddScoped<IChannelService, ChannelServiceImpl>();
            services.AddScoped<ICategoryService, CategoryServiceImpl>();
            //services.AddScoped<IPostService, PostServiceImpl>();
            services.AddScoped<ILikesService, LikesServiceImpl>();
            services.AddScoped<IStripeService, StripeServiceImpl>();
            services.AddSingleton<WebsocketHandler, WebsocketHandler>();

            /**
             * inject Appseting into DI container
             */
            services.AddSingleton(new AppSettings(configuration));
        }
    }
}
