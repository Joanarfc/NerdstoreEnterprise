using Microsoft.Extensions.DependencyInjection;
using NSE.Identity.API.Services;
using NSE.WebAPI.Core.Utilizador;

namespace NSE.Identity.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<AuthenticationService>();
            services.AddScoped<IAspNetUser, AspNetUser>();
        }
    }
}