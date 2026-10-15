using MapeamentoUrbano.API.Applications;
using MapeamentoUrbano.API.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MapeamentoUrbano.API.DependencyInjection
{
    public class ConfigureServices
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<EventApp>();
            services.AddTransient<JWTKeyApp>();
            services.AddTransient<LogApp>();
            services.AddTransient<LoginApp>();
            services.AddTransient<UserApp>();
            services.AddTransient<ValidationApp>();
            services.AddTransient<PasswordHelper>();

            #region [Auth]
            string? token = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            if (string.IsNullOrEmpty(token))
                token = configuration.GetSection("JwtSettings")["JwtSecretKey"];

            var key = Encoding.ASCII.GetBytes(token);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            #endregion

            services.AddCors();
            services.AddControllers();
        }
    }
}
