using Api.Extensions;
using Business.Intefaces;
using Business.Notificacoes;
using Business.Services;
using Data.Context;
using Data.Repository;

namespace Api.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            // Contexto do DB
            services.AddScoped<MeuDbContext>();

            // Repositorios
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();

            // Serviços
            services.AddScoped<IFornecedorService, FornecedorService>();
            services.AddScoped<IProdutoService, ProdutoService>();

            // Notificador
            services.AddScoped<INotificador, Notificador>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUser, AspNetUser>();

            return services;
        }
    }
}