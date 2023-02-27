using Microsoft.Extensions.DependencyInjection;
using NSE.Core.Mediator;
using NSE.Orders.Infra.Data;

namespace NSE.Orders.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<PedidosContext>();
            services.AddScoped<IMediatorHandler, MediatorHandler>();
        }
    }
}