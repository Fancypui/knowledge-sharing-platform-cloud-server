using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Models.Comment;
using knowledge_sharing_platform_cloud.Data.Models.Channel;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Services;
using knowledge_sharing_platform_cloud.Services.impl;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using knowledge_sharing_platform_cloud.Data.Models.ChannelMember;
using knowledge_sharing_platform_cloud.Websocket;
using knowledge_sharing_platform_cloud.Models.DTO;
using knowledge_sharing_platform_cloud.Services.Consumer;
using knowledge_sharing_platform_cloud.Data.Models.Category;
using Amazon.S3;
using knowledge_sharing_platform_cloud.Data.Models.Post;
using knowledge_sharing_platform_cloud.Data.Models.Likes;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
//builder.Services.AddDbContext<ApplicationDBContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("default"))
//    );

/**
 * service dependency injection
 */
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("redis");
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<CommentCache,CommentCache>();
builder.Services.AddScoped<UserCache,UserCache>();
builder.Services.AddScoped<ChannelSummaryCache,ChannelSummaryCache>();
builder.Services.AddScoped<ICommentSerivce, CommentServiceImpl>();
builder.Services.AddSingleton<IWebsocketService, WebsocketServiceImpl>();
builder.Services.AddScoped<UserRepo, UserRepo>();
builder.Services.AddScoped<CommentRepo, CommentRepo>();
builder.Services.AddScoped<UserContext, UserContext>();
builder.Services.AddScoped<CommentContext, CommentContext>();


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<UserContext>();
builder.Services.AddTransient<UserRepo>();
builder.Services.AddTransient<IUserService, UserServiceImpl>();

builder.Services.AddTransient<ChannelRepo>();
builder.Services.AddTransient<ChannelContext>();
builder.Services.AddTransient<IChannelService, ChannelServiceImpl>();

builder.Services.AddTransient<CategoryRepo>();
builder.Services.AddTransient<CategoryContext>();
builder.Services.AddTransient<ICategoryService, CategoryServiceImpl>();

builder.Services.AddTransient<PostRepo>();
builder.Services.AddTransient<PostContext>();
builder.Services.AddTransient<IPostService, PostServiceImpl>();

builder.Services.AddTransient<LikesRepo>();
builder.Services.AddTransient<LikesContext>();
builder.Services.AddTransient<ILikesService, LikesServiceImpl>();


builder.Services.AddTransient<ChannelMemberRepo>();
builder.Services.AddTransient<ChannelMemberContext>();

builder.Services.AddTransient<IStripeService, StripeServiceImpl>();

builder.Services.AddTransient<S3Service>();



/*
 * Websocket handler dependency injection
 * use to handle websocket related operation
 */
builder.Services.AddSingleton<WebsocketHandler,WebsocketHandler>();

/**
 * SNS SQS Config
 */
var awsOptions = new Amazon.Extensions.NETCore.Setup.AWSOptions
{
    Region = Amazon.RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"]),
    Credentials = new Amazon.Runtime.SessionAWSCredentials(
        builder.Configuration["AWS:AccessKey"],
        builder.Configuration["AWS:SecretKey"],
        builder.Configuration["AWS:SessionToken"])
};
//builder.Services.AddDefaultAWSOptions(awsOptions);
//var snsPushNotificationARN = builder.Configuration.GetValue<string>("AWS:SNSPushNotificationArn");
//var sqsPushNotificationQueueARN = builder.Configuration.GetValue<string>("AWS:SQSPushNotificationQueueARN");
//var sqsRedisChannelLeaderBoardARN = builder.Configuration.GetValue<string>("AWS:SQSRedisChannelLeaderBoardARN");

//builder.Services.AddAWSMessageBus(builder =>
//{
//    /**
//     * register sns publisher (push notification topic)
//     */
//    builder.AddSNSPublisher<PushNotificationDTO>(snsPushNotificationARN);
//    /**
//     * register sqs publisher (queue to update channel leaderboard in redis)
//     */
//    builder.AddSQSPublisher<ChannelLeaderboardDTO>(sqsRedisChannelLeaderBoardARN);
//    /**
//     * register sqs queue (push notification queue) to poll messages
//     */
//    builder.AddSQSPoller(sqsPushNotificationQueueARN, options =>
//    {
//        // The maximum number of messages from this queue that the framework will process concurrently on this client
//        options.MaxNumberOfConcurrentMessages = 10;

//        // The duration each call to SQS will wait for new messages
//        options.WaitTimeSeconds = 20;
//    });
//    /**
//     * register sqs queue (event to update channel leaderboard in redis) to poll messages
//     */
//    builder.AddSQSPoller(sqsRedisChannelLeaderBoardARN, options =>
//    {
//        // The maximum number of messages from this queue that the framework will process concurrently on this client
//        options.MaxNumberOfConcurrentMessages = 10;

//        // The duration each call to SQS will wait for new messages
//        options.WaitTimeSeconds = 20;
//    });
//    builder.AddMessageHandler<PushNotificationConsumer, PushNotificationDTO> ();

//});


// setup AWS S3
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonS3>();

var app = builder.Build();
app.UseExceptionHandler(options => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
/**
 * ping frame to client every two minutes
 */
var websocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
/**
 * register websocket middleware
 */
app.UseWebSockets(websocketOptions);

app.MapControllers();

app.Run();
