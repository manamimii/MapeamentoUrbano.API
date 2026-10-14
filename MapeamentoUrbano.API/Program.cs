using MapeamentoUrbano.API.DependencyInjection;
using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Data.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ConfigureRepositories.Configure(
    builder.Services,
    builder.Configuration
);

ConfigureServices.Configure(
    builder.Services,
    builder.Configuration
);

builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles
    );

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document
                ),
                new List<string>()
            }
        });
});

var app = builder.Build();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    using var context = serviceScope.ServiceProvider.GetService<CoreContext>();
    context.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MapeamentoUrbano API");

    c.DocumentTitle = "MapeamentoUrbano API";
    c.EnableValidator(null);
    c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
});

app.UseHttpsRedirection();

app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();