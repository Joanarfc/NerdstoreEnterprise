using Microsoft.Extensions.Options;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public interface IAutenticacaoService
    {
        Task<UsuarioRespostaLogin> Registo(UsuarioRegisto usuarioRegisto);
        Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin);
    }

    public class AutenticacaoService : Service, IAutenticacaoService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _settings;

        public AutenticacaoService(HttpClient httpClient, 
                                  IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }
        public async Task<UsuarioRespostaLogin> Registo(UsuarioRegisto usuarioRegisto)
        {
            var loginContent = ObterConteudo(usuarioRegisto);

            var response = await _httpClient.PostAsync($"{_settings.AutenticacaoUrl}/api/identidade/nova-conta", loginContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)

                };
            }
            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }
        public async Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin)
        {
            var loginContent = ObterConteudo(usuarioLogin);

            var response = await _httpClient.PostAsync($"{_settings.AutenticacaoUrl}/api/identidade/autenticar", loginContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)

                };
            }

            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }
    }
}
