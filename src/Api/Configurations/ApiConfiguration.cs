using Microsoft.AspNetCore.Mvc;

namespace Api.Configurations
{
    public static class ApiConfiguration
    {
        public static IServiceCollection SetApiConfiguration(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // Define a nomenclatura do versionamento
                options.SubstituteApiVersionInUrl = true;
            });

            // Desabilita a validação que o asp.net faz nas models com base nas dataannotations
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            return app;
        }
    }
}
