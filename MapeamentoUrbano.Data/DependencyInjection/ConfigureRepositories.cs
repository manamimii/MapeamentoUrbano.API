using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Data.Repository;
using MapeamentoUrbano.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MapeamentoUrbano.Data.DependencyInjection
{
    public class ConfigureRepositories
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = "Host=ep-broad-dawn-b6kpkah9-pooler.c-2.sa-east-1.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_U4YXRsBZ7jGg;SSL Mode=Require;Channel Binding=Require";

            if (connectionString != null)
                services.AddDbContext<CoreContext>(options => options.UseNpgsql(connectionString));

            else
                services.AddDbContext<CoreContext>(options => options.UseNpgsql(configuration.GetConnectionString("ContextDataSource")));

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        }
    }
}
