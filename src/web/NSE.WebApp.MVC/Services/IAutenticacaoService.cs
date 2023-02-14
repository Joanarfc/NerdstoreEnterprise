using NSE.WebApp.MVC.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public interface IAutenticacaoService
    {
        Task<string> Registo(UsuarioRegisto usuarioRegisto);
        Task<string> Login(UsuarioLogin usuarioLogin);
    }

    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly HttpClient _httpClient;

        public AutenticacaoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> Registo(UsuarioRegisto usuarioRegisto)
        {
            var loginContent = new StringContent(JsonSerializer.Serialize(usuarioRegisto), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:44363/api/identidade/nova-conta", loginContent);

            return JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync());
        }
        public async Task<string> Login(UsuarioLogin usuarioLogin)
        {
            var loginContent = new StringContent(JsonSerializer.Serialize(usuarioLogin), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:44363/api/identidade/autenticar", loginContent);

            var teste = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync());
        }
    }
}
