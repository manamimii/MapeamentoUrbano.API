using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MapeamentoUrbano.Domain.Helpers
{
    public class TokenHelper
    {
        public static Guid GetUserId(HttpContext? httpContext)
        {
            if (httpContext?.Request?.Headers == null || 
                !httpContext.Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues token))
                return Guid.Empty;

            if (string.IsNullOrWhiteSpace(token))
                return Guid.Empty;

            Claim[]? claims = DeserializeJwtToken(token);
            if (claims != null && claims.Any())
            {
                Claim? userIdClaim = claims.FirstOrDefault(c => c.Type == "Id");
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return userId;
                }
            }

            return Guid.Empty;
        }

        public static Claim[]? DeserializeJwtToken(string token)
        {
            if (token.Contains("Bearer "))
                token = token.Split(" ")[1];

            JwtSecurityTokenHandler tokenHandler = new();

            return tokenHandler.ReadJwtToken(token).Claims.ToArray();
        }
    }
}
