using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Controllers
{
    public class IdentidadeController : Controller
    {
        private readonly IAutenticacaoService _autenticacaoService;

        public IdentidadeController(IAutenticacaoService autenticacaoService)
        {
            _autenticacaoService = autenticacaoService;
        }

        [HttpGet]
        [Route("nova-conta")]
        public IActionResult Registo()
        {
            return View();
        }

        [HttpPost]
        [Route("nova-conta")]
        public async Task<IActionResult> Registo(UsuarioRegisto usuarioRegisto)
        {
            if(!ModelState.IsValid) return View(usuarioRegisto);

            // API - User registration
            var resposta = await _autenticacaoService.Registo(usuarioRegisto);

            if (false) return View(usuarioRegisto);

            // Execute login in the app
            await RealizarLogin(resposta);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return View(usuarioLogin);

            // API - User login

            var resposta = await _autenticacaoService.Login(usuarioLogin);

            if (false) return View(usuarioLogin);

            // Execute login in the app
            await RealizarLogin(resposta);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("sair")]
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }

        public async Task RealizarLogin(UsuarioRespostaLogin resposta)
        {
            /* Get the AccessToken from 'resposta' converted in JwtSecurityToken format */
            var token = ObterTokenFormatado(resposta.AccessToken);

            var claims = new List<Claim>();

            claims.Add(new Claim("JWT", resposta.AccessToken));

            claims.AddRange(token.Claims);

            /* The ClaimsIdentity object contains the claims collection + the Cookie Authentication Scheme */
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        private static JwtSecurityToken ObterTokenFormatado(string jwtToken)
        {
            return new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken;
        }
    }
}
