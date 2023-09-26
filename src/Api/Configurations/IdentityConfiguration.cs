using Api.Data;
using Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.Configurations
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()     
                .AddErrorDescriber<IdentityMensagensPortugues>()
                .AddDefaultTokenProviders();

            // JWT //

            var jwtSettingsSection = configuration.GetSection("JwtSettings"); // Pega a sessão 'JwtSettings' do arquivo appsettings.json
            services.Configure<JwtSettings>(jwtSettingsSection); // Dizendo que a class JwtSettings representa um trecho do appsettings.json

            var jwtSettings = jwtSettingsSection.Get<JwtSettings>(); // Pegando os dados da classe JwtSettings. (Os dados vieram do appsettings.json)
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret); // Fazendo o Enconding com base na Secret 

            services.AddAuthentication(x => 
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Sempre que for autenticar alguem, vai ser usado um token
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Ao fazer a validação, fará com base no token
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true; // Obriga o uso de conexões https
                x.SaveToken = true; // Armazena o token após a autenticação ter sido bem sucedida
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Criptografa a chave
                    ValidateIssuer = true, // Valida o emissor. (Deve ser o mesmo que está no appsettings.json)
                    ValidateAudience = true, // Valida o endereço. (Deve ser o mesmo que está no appsettings.json)
                    ValidAudience = jwtSettings.ValidoEm,
                    ValidIssuer = jwtSettings.Emissor
                };
            });

            return services;
        }
    }
}
