using Api.Configurations;
using Data.Context;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<MeuDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.SetApiConfiguration();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.ResolveDependencies();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();

builder.Services.AddLoggerConfiguration();

builder.Services
    .AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), name: "BancoSQL"); // Verifica se o banco está on

builder.Services
    .AddHealthChecksUI(options => options.AddHealthCheckEndpoint("API com Health Checks", "/api/hc"))
    .AddInMemoryStorage();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfiguration(app.Services.GetRequiredService<IApiVersionDescriptionProvider>());
}

app.UseApiConfiguration();

app.MapControllers();

app.UseLoggerConfiguration(builder);

app.UseHealthChecks("/api/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(options => 
{
    options.UIPath = "/api/hc-ui";
    options.UseRelativeApiPath = false;
    options.UseRelativeResourcesPath = false;
});

app.Run();