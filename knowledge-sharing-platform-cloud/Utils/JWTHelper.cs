using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using knowledge_sharing_platform_cloud.Config;
using knowledge_sharing_platform_cloud.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace knowledge_sharing_platform_cloud.Utils
{
    public class JWTHelper
    {
        private static readonly int EXPIRY_DURATION = 30;

        public static string IssueToken(long uid)
        {
            /**
             * load info from appsetting
             */
            string secret = AppSettings.GetVal("JWT:Secret");
            string issuer = AppSettings.GetVal("JWT:Issuer");
            string audience = AppSettings.GetVal("JWT:Audience");

            JwtSecurityTokenHandler tokenHanlder = new JwtSecurityTokenHandler();

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            /**
             * expiry date
             */
            DateTime expiresAt = DateTime.Now.AddDays(EXPIRY_DURATION);
            /**
             * put user info into claims
             */
            var claimsIdentity = new Dictionary<string, object> 
            {
                { JwtRegisteredClaimNames.Jti, uid.ToString() },
                { JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() }
            };
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = claimsIdentity,
                Issuer = issuer,
                Audience = audience,
                NotBefore = DateTime.Now,
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };
            SecurityToken token = tokenHanlder.CreateToken(tokenDescriptor);
            return tokenHanlder.WriteToken(token);
        }

        
    }
}
