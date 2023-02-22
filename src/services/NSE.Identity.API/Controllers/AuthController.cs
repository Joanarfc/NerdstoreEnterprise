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
using EasyNetQ;

namespace NSE.Identity.API.Controllers
{
    [Route("api/identidade")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;
        private IBus _bus;

        public AuthController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IOptions<AppSettings> appSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
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
                // Integration
                var sucesso = await RegistarCliente(usuarioRegisto);

                return CustomResponse(await GerarJwt(usuarioRegisto.Email));
            }

            foreach (var error in result.Errors)
            {
                AdicionarErroProcessamento(error.Description);
            }

            return CustomResponse();
        }

        public async Task<ResponseMessage> RegistarCliente(UsuarioRegisto usuarioRegisto)
        {
            var utilizador = await _userManager.FindByEmailAsync(usuarioRegisto.Email);

            var utilizadorRegistado = new UtilizadorRegistadoIntegrationEvent(
                Guid.Parse(utilizador.Id), usuarioRegisto.Nome, usuarioRegisto.Email, usuarioRegisto.Cpf);

            _bus = RabbitHutch.CreateBus("host=localhost:5672");

            var sucesso = await _bus.Rpc.RequestAsync<UtilizadorRegistadoIntegrationEvent, ResponseMessage>(utilizadorRegistado);

            return sucesso;
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
            var encodedToken = CodificarToken(identityClaims);


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
        private string CodificarToken(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            /* JWT generation based on:
               the values we defined in the appsettings.json file for the Emissor (Issuer), ValidoEm (Audience) and ExpiracaoHoras (Expires)
               the ClaimsIdentity object (Subject)
               and the security key (SigningCredentials) */
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
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
    }
}
