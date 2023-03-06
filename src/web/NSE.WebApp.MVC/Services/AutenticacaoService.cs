using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using NSE.Core.Communication;
using NSE.WebAPI.Core.Utilizador;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public interface IAutenticacaoService
    {
        Task<UsuarioRespostaLogin> Registo(UsuarioRegisto usuarioRegisto);
        Task<UsuarioRespostaLogin> Login(UsuarioLogin usuarioLogin);
        Task RealizarLogin(UsuarioRespostaLogin resposta);
        Task Logout();
        bool TokenExpirado();
        Task<bool> RefreshTokenValido();
    }

    public class AutenticacaoService : Service, IAutenticacaoService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAspNetUser _user;

        public AutenticacaoService(HttpClient httpClient,
                                  IOptions<AppSettings> settings,
                                  IAuthenticationService authenticationService,
                                  IAspNetUser user)
        {
            httpClient.BaseAddress = new Uri(settings.Value.AutenticacaoUrl);
            _httpClient = httpClient;
            _authenticationService = authenticationService;
            _user = user;
        }
        public async Task<UsuarioRespostaLogin> Registo(UsuarioRegisto usuarioRegisto)
        {
            var loginContent = ObterConteudo(usuarioRegisto);

            var response = await _httpClient.PostAsync("/api/identidade/nova-conta", loginContent);

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

            var response = await _httpClient.PostAsync("/api/identidade/autenticar", loginContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)

                };
            }

            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }
        public async Task Logout()
        {
            await _authenticationService.SignOutAsync(
                _user.ObterHttpContext(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                null);
        }
        public async Task<UsuarioRespostaLogin> UtilizarRefreshToken(string refreshToken)
        {
            var refreshTokenContent = ObterConteudo(refreshToken);

            var response = await _httpClient.PostAsync("/api/identidade/refresh-token", refreshTokenContent);

            if (!TratarErrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }
            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }
        public async Task RealizarLogin(UsuarioRespostaLogin resposta)
        {
            /* Get the AccessToken from 'resposta' converted in JwtSecurityToken format */
            var token = ObterTokenFormatado(resposta.AccessToken);

            var claims = new List<Claim>();

            claims.Add(new Claim("JWT", resposta.AccessToken));
            claims.Add(new Claim("RefreshToken", resposta.RefreshToken));

            claims.AddRange(token.Claims);

            /* The ClaimsIdentity object contains the claims collection + the Cookie Authentication Scheme */
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                IsPersistent = true
            };

            await _authenticationService.SignInAsync(
                _user.ObterHttpContext(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
        public static JwtSecurityToken ObterTokenFormatado(string jwtToken)
        {
            return new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken;
        }
        public bool TokenExpirado()
        {
            var jwt = _user.ObterUserToken();

            if (jwt is null) return false;

            var token = ObterTokenFormatado(jwt);
            return token.ValidTo.ToLocalTime() < DateTime.Now;
        }
        public async Task<bool> RefreshTokenValido()
        {
            var resposta = await UtilizarRefreshToken(_user.ObterUserRefreshToken());

            if (resposta.AccessToken != null && resposta.ResponseResult == null)
            {
                await RealizarLogin(resposta);
                return true;
            }

            return false;
        }
    }
}