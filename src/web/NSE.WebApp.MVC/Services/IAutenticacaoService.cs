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

        public AutenticacaoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<UsuarioRespostaLogin> Registo(UsuarioRegisto usuarioRegisto)
        {
            var loginContent = ObterConteudo(usuarioRegisto);

            var response = await _httpClient.PostAsync("https://localhost:44363/api/identidade/nova-conta", loginContent);

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

            var response = await _httpClient.PostAsync("https://localhost:44363/api/identidade/autenticar", loginContent);

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
