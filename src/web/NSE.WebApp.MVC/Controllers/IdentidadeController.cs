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
    public class IdentidadeController : MainController
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
            if (!ModelState.IsValid) return View(usuarioRegisto);

            // API - User registration
            var resposta = await _autenticacaoService.Registo(usuarioRegisto);

            /* If the response from the registration through the service API contains errors, it will return to the View */
            if (ResponsePossuiErros(resposta.ResponseResult)) return View(usuarioRegisto);

            // Execute login in the app
            await _autenticacaoService.RealizarLogin(resposta);

            return RedirectToAction("Index", "Catalogo");
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UsuarioLogin usuarioLogin, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) return View(usuarioLogin);

            // API - User login

            var resposta = await _autenticacaoService.Login(usuarioLogin);

            /* If the response from the login through the service API contains errors, it will return to the View */
            if (ResponsePossuiErros(resposta.ResponseResult)) return View(usuarioLogin);

            // Execute login in the app
            await _autenticacaoService.RealizarLogin(resposta);

            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction("Index", "Catalogo");

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        [Route("sair")]
        public async Task<IActionResult> Logout()
        {
            await _autenticacaoService.Logout();

            return RedirectToAction("Index", "Catalogo");
        }
    }
}
