using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Catalog.API.Models;
using NSE.WebAPI.Core.Identidade;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSE.Catalog.API.Controllers
{
    [ApiController]
    [Authorize]
    public class CatalogController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;

        public CatalogController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        [AllowAnonymous]
        [HttpGet("catalogo/produtos")]
        public async Task<IEnumerable<Produto>> Index()
        {
            return await _produtoRepository.ObterTodos();
        }

        [ClaimsAuthorize("Catalogo","Ler")]
        [HttpGet("catalogo/produtos/{id:guid}")]
        public async Task<Produto> ProdutoDetalhe(Guid id)
        {
            return await _produtoRepository.ObterPorId(id);
        }
    }
}