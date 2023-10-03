using KissLog.AspNetCore;
using KissLog.CloudListeners.Auth;
using KissLog.CloudListeners.RequestLogsListener;
using KissLog.Formatters;

namespace Api.Configurations
{
    public static class LoggerConfiguration
    {
        public static IServiceCollection AddLoggerConfiguration(this IServiceCollection services)
        {
            services.AddLogging(provider =>
            {
                provider
                    .AddKissLog(options =>
                    {
                        options.Formatter = (FormatterArgs args) =>
                        {
                            if (args.Exception == null) return args.DefaultValue;
                            var exception = new ExceptionFormatter().Format(args.Exception, args.Logger);
                            return string.Join(Environment.NewLine, new[] { args.DefaultValue, exception });
                        };
                    });
            });

            services.AddHttpContextAccessor();

            return services;
        }

        public static IApplicationBuilder UseLoggerConfiguration(this IApplicationBuilder app, WebApplicationBuilder builder)
        {
            app.UseKissLogMiddleware(options => {
                options.Listeners.Add(
                    new RequestLogsApiListener(new Application(
                        builder.Configuration["KissLog.OrganizationId"], 
                        builder.Configuration["KissLog.ApplicationId"]))
                    {
                        ApiUrl = builder.Configuration["KissLog.ApiUrl"]
                    }
                );
            });

            return app;
        }
    }
}