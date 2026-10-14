using MapeamentoUrbano.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MapeamentoUrbano.API.Applications
{
    public class JWTKeyApp
    {
        private readonly string? _jwtSecretKey;
        private readonly IConfiguration _configuration;

        public JWTKeyApp(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSecretKey = _configuration.GetValue<string>("JwtSettings:JwtSecretKey");
        }

        #region [Generate Token]
        internal string GenerateUserToken(User user)
        {
            try
            {
                var claims = new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role.RoleName),
                    new Claim("FirstAccess", user.FirstAccess.ToString())
                };

                return GenerateJwtToken(claims);
            }
            catch (Exception ex) 
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
        #endregion

        #region [Renew Token]
        internal string RenewToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = GetTokenValidationParameters(false);

            try
            {
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                var claims = claimsPrincipal.Claims.Take(3).ToArray();

                return GenerateJwtToken(claims);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region [Validate Token]
        internal int ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenValidationParameters = GetTokenValidationParameters(true);

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return StatusCodes.Status401Unauthorized;

                return StatusCodes.Status200OK;
            }
            catch (SecurityTokenExpiredException)
            {
                return StatusCodes.Status401Unauthorized;
            }
            catch (SecurityTokenValidationException)
            {
                return StatusCodes.Status401Unauthorized;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region [Helper Methods]
        private TokenValidationParameters GetTokenValidationParameters(bool validateLifetime)
        {
            return new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = validateLifetime,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecretKey))
            };
        }

        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(10),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion
    }
}
