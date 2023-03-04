using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSE.Identity.API.Models;
using System.Threading.Tasks;
using NSE.WebAPI.Core.Controllers;
using NSE.MessageBus;
using NSE.Identity.API.Services;
using NSE.Core.Messages.Integration;
using System;

namespace NSE.Identity.API.Controllers
{
    [Route("api/identidade")]
    public class AuthController : MainController
    {
        private readonly AuthenticationService _authenticationService;
        private readonly IMessageBus _bus;

        public AuthController(AuthenticationService authenticationService, 
                              IMessageBus bus)
        {
            _authenticationService = authenticationService;
            _bus = bus;
        }

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registar(UsuarioRegisto usuarioRegisto)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = usuarioRegisto.Email,
                Email = usuarioRegisto.Email,
                EmailConfirmed = true
            };

            var result = await _authenticationService.UserManager.CreateAsync(user, usuarioRegisto.Senha);

            if (result.Succeeded)
            {
                // Integration: if everything went well, we will create the client
                var clientResult = await RegistarCliente(usuarioRegisto);

                if(!clientResult.ValidationResult.IsValid)
                {
                    // In case we couldn't create the client, we will then delete the user
                    await _authenticationService.UserManager.DeleteAsync(user);
                    return CustomResponse(clientResult.ValidationResult);
                }

                return CustomResponse(await _authenticationService.GerarJwt(usuarioRegisto.Email));
            }

            foreach (var error in result.Errors)
            {
                AdicionarErroProcessamento(error.Description);
            }

            return CustomResponse();
        }
        
        [HttpPost("autenticar")]
        public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _authenticationService.SignInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true);

            if (result.Succeeded)
            {
                return CustomResponse(await _authenticationService.GerarJwt(usuarioLogin.Email));
            }
            if(result.IsLockedOut)
            {
                AdicionarErroProcessamento("Utilizador temporariamente bloqueado por tentativas inválidas.");
                return CustomResponse();
            }

            AdicionarErroProcessamento("Utilizador ou senha incorretos");

            return CustomResponse();
        }
        private async Task<ResponseMessage> RegistarCliente(UsuarioRegisto UsuarioRegisto)
        {
            var usuario = await _authenticationService.UserManager.FindByEmailAsync(UsuarioRegisto.Email);

            var usuarioRegistrado = new UtilizadorRegistadoIntegrationEvent(
                Guid.Parse(usuario.Id), UsuarioRegisto.Nome, UsuarioRegisto.Email, UsuarioRegisto.Cpf);

            try
            {
                return await _bus.RequestAsync<UtilizadorRegistadoIntegrationEvent, ResponseMessage>(usuarioRegistrado);
            }
            catch
            {
                await _authenticationService.UserManager.DeleteAsync(usuario);
                throw;
            }
        }
        
        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                AdicionarErroProcessamento("Refresh Token inválido");
                return CustomResponse();
            }

            var token = await _authenticationService.ObterRefreshToken(Guid.Parse(refreshToken));

            if (token is null)
            {
                AdicionarErroProcessamento("Refresh Token expirado");
                return CustomResponse();
            }

            return CustomResponse(await _authenticationService.GerarJwt(token.Username));
        }
    }
}