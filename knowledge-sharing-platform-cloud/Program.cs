using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.config;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Models.Comment;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Services;
using knowledge_sharing_platform_cloud.Services.impl;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

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
builder.Services.AddScoped<ICommentSerivce, CommentServiceImpl>();
builder.Services.AddScoped<UserRepo, UserRepo>();
builder.Services.AddScoped<CommentRepo, CommentRepo>();
builder.Services.AddScoped<UserContext, UserContext>();
builder.Services.AddScoped<CommentContext, CommentContext>();


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<UserContext>();
builder.Services.AddTransient<IUserRepo, UserRepo>();

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

app.MapControllers();

app.Run();
