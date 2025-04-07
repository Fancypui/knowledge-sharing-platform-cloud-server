using System.Runtime.CompilerServices;
using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Services.impl;
using knowledge_sharing_platform_cloud.Services;
using StackExchange.Redis;

using knowledge_sharing_platform_cloud.Data.Repositories;

using knowledge_sharing_platform_cloud.Websocket;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using Microsoft.AspNetCore.Mvc;
using knowledge_sharing_platform_cloud.Data.Constant;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Services.Consumer;

namespace knowledge_sharing_platform_cloud.Config
{
    public static class ServiceConfig
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
  
            string currentDirectory = Directory.GetCurrentDirectory();
            string luaFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Cache", "ChannelLeaderboardUpdate.lua");
            bool exist = File.Exists(luaFilePath);
            string _luaScript = File.ReadAllText(luaFilePath);
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
            services.AddScoped<PostImgUrlsCache, PostImgUrlsCache>();
            services.AddScoped<UserCache, UserCache>();
            services.AddScoped<ChannelSummaryCache, ChannelSummaryCache>();
            services.AddScoped<ChannelLeaderboardCache, ChannelLeaderboardCache>();
            /**
             * add controllers, swagger, endpoint config
             */
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(m => m.Value.Errors.Any())
                            .ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                            );
                        string errorString = string.Join("; ", errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"));
                        var response = ApiResult<string>.ServiceFail((int)CommonErrorEnum.BUSINESS_ERROR, errorString);

                        return new BadRequestObjectResult(response);
                    };
                });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            /**
             * repo and context dependency injection
             */
            services.AddScoped<ApplicationContext, ApplicationContext>();
            services.AddScoped<UserRepo, UserRepo>();
            services.AddScoped<CommentRepo, CommentRepo>();
            services.AddScoped<PostRepo, PostRepo>();
            services.AddScoped<ChannelRepo>();
            services.AddScoped<CategoryRepo>();
            services.AddTransient<LikesRepo>();
            services.AddTransient<ChannelMemberRepo>();
        

            /**
             * services dependency injection
             */
            services.AddScoped<ICommentSerivce, CommentServiceImpl>();
            services.AddSingleton<IWebsocketService, WebsocketServiceImpl>();
            services.AddScoped<IUserService, UserServiceImpl>();
            services.AddScoped<IChannelService, ChannelServiceImpl>();
            services.AddScoped<ICategoryService, CategoryServiceImpl>();
            services.AddScoped<IPostService, PostServiceImpl>();
            services.AddScoped<ILikesService, LikesServiceImpl>();
            services.AddScoped<IStripeService, StripeServiceImpl>();
            services.AddSingleton<WebsocketHandler, WebsocketHandler>();
            services.AddScoped<IWebPushService,WebPushServiceImpl>();
            services.AddHostedService<StripePaymentWebhookConsumer>();

            /**
             * inject Appseting into DI container
             */
            services.AddSingleton(new AppSettings(configuration));
        }
    }
}
