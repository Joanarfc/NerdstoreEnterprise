using Microsoft.Extensions.DependencyInjection;
using NSE.Payments.API.Data;

namespace NSE.Payments.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<PagamentosContext>();
        }
    }
}