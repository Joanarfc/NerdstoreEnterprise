using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NSE.Identity.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using NSE.WebAPI.Core.Identidade;
using NSE.WebAPI.Core.Controllers;
using NSE.Core.Messages.Integration;
using NSE.MessageBus;
using NSE.WebAPI.Core.Utilizador;
using NetDevPack.Security.Jwt.Core.Interfaces;

namespace NSE.Identity.API.Controllers
{
    [Route("api/identidade")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly IMessageBus _bus;
        private readonly IAspNetUser _user;
        private readonly IJwtService _jwtService;

        public AuthController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IOptions<AppSettings> appSettings,
                              IMessageBus bus,
                              IAspNetUser user,
                              IJwtService jwtService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _bus = bus;
            _user = user;
            _jwtService = jwtService;
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

            var result = await _userManager.CreateAsync(user, usuarioRegisto.Senha);

            if (result.Succeeded)
            {
                // Integration: if everything went well, we will create the client
                var clientResult = await RegistarCliente(usuarioRegisto);

                if(!clientResult.ValidationResult.IsValid)
                {
                    // In case we couldn't create the client, we will then delete the user
                    await _userManager.DeleteAsync(user);
                    return CustomResponse(clientResult.ValidationResult);
                }

                return CustomResponse(await GerarJwt(usuarioRegisto.Email));
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

            var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true);

            if (result.Succeeded)
            {
                return CustomResponse(await GerarJwt(usuarioLogin.Email));
            }
            if(result.IsLockedOut)
            {
                AdicionarErroProcessamento("Utilizador temporariamente bloqueado por tentativas inválidas.");
                return CustomResponse();
            }

            AdicionarErroProcessamento("Utilizador ou senha incorretos");

            return CustomResponse();
        }
        private async Task<UsuarioRespostaLogin> GerarJwt(string email)
        {
            /* Retrieve the user information (claims and roles) associated with the email using the _userManager  */
            var user = await _userManager.FindByNameAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);

            /* Obtain the user claims */
            var identityClaims = await ObterClaimsUsuario(claims, user);
            
            /* Generate the encoded token */
            var encodedToken = await CodificarToken(identityClaims);


            return ObterRespostaToken(encodedToken, user, claims); 
        }
        private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            /* Add claims with additional information: user Id (Sub), Email, JWT unique identifier (Jti)
               and the time of token issuance (NBF and IAT) */
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }
        private async Task<string> CodificarToken(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var currentIssuer = $"{_user.ObterHttpContext().Request.Scheme}://{_user.ObterHttpContext().Request.Host}";

            var key = await _jwtService.GetCurrentSigningCredentials();

            /* JWT generation based on:
               the values we defined in the appsettings.json file for the Emissor (Issuer), ValidoEm (Audience) and ExpiracaoHoras (Expires)
               the ClaimsIdentity object (Subject)
               and the security key (SigningCredentials) */
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = currentIssuer,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = key
            });

            return tokenHandler.WriteToken(token);
        }
        private UsuarioRespostaLogin ObterRespostaToken(string encodedToken, IdentityUser user, IEnumerable<Claim> claims)
        {
            /* Generation of a UsuarioRespostaLogin object containing the encoded token, its expiration time
               and the user information (Id, email and Claims) associated with the token */
            return new UsuarioRespostaLogin
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
                UsuarioToken = new UsuarioToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
                }
            };
        }

        /* Calculate the number of seconds elapsed since the Unix epoch (January 1, 1970, 00:00:00 UTC) to a given date */
        private static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0,
              TimeSpan.Zero)).TotalSeconds);
        private async Task<ResponseMessage> RegistarCliente(UsuarioRegisto usuarioRegisto)
        {
            var utilizador = await _userManager.FindByEmailAsync(usuarioRegisto.Email);

            var utilizadorRegistado = new UtilizadorRegistadoIntegrationEvent(
                Guid.Parse(utilizador.Id), usuarioRegisto.Nome, usuarioRegisto.Email, usuarioRegisto.Cpf);

            try
            {
                return await _bus.RequestAsync<UtilizadorRegistadoIntegrationEvent, ResponseMessage>(utilizadorRegistado);
            }
            catch
            {
                await _userManager.DeleteAsync(utilizador);
                throw;
            }
        }
    }
}