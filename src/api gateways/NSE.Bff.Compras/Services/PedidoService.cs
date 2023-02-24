using Microsoft.Extensions.Options;
using NSE.Bff.Compras.Extensions;
using System.Net.Http;
using System;

namespace NSE.Bff.Compras.Services
{
    public interface IPedidoService
    {
    }
    public class PedidoService : Service, IPedidoService
    {
        private readonly HttpClient _httpClient;

        public PedidoService(HttpClient httpClient,
                               IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.PedidoUrl);
        }
    }
}