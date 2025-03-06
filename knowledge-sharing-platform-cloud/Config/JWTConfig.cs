using System.Security.Claims;
using System.Text;
using System.Text.Json;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace knowledge_sharing_platform_cloud.Config
{
    public static  class JWTConfig
    {
        public static void AddJwtAuthentication(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:Secret"])),
                    ClockSkew = TimeSpan.Zero,
                };
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Extract the Jti claim (user ID) from the token
                        var userIdClaim = context.Principal.FindFirst(JwtRegisteredClaimNames.Jti);

                        if (userIdClaim != null)
                        {
                            // Store the user ID in HttpContext.Items
                            context.HttpContext.Items["UserId"] = userIdClaim.Value;
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = async context =>
                    {
                        // Skip the default 401 response so we can return our own
                        context.HandleResponse();
                        // Set response status and content type
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        // Custom response body
                        var response = ApiResult<string>.ServiceFail((int)CommonErrorEnum.UNAUTHORIZED, "Unauthorized access. Token is invalid or missing.");
                        var jsonResponse = JsonSerializer.Serialize(response);

                        await context.Response.WriteAsync(jsonResponse);
                    }
                };
            });
        }
    }
}
