using Autenticacao.Services.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Triagem.Service.IRepository;

namespace Triagem.Controllers
{
    [Route("api/distirbuicao/v1")]
    [ApiController]
    public class RedistribuicaoController : ControllerBase
    {
        private readonly IRedistribuicaoRepository _service;

        public RedistribuicaoController(IRedistribuicaoRepository services) => _service = services;


        [HttpGet("localizarprocessos")]
        [CustomAuthorize("Administrador")]
        public async Task<IActionResult> LocalizarProcessos()
        {
            try
            {
                var usuario = await _service.BuscarProcessoUsuario();

                if (usuario == null)
                {
                    return BadRequest(new { erro = 400, mensagem = "Não existe dados para essa pesquisa" });
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        [HttpPost("redistribuir")]
        [CustomAuthorize("Administrador")]
        public async Task<IActionResult> Redistribuir(string usuarioorigem, string usuariodestino, int quantidade)
        {
            try
            {
                await _service.RedistribuicaoProcesso(usuarioorigem, usuariodestino, quantidade);
                Log.Information("Usuário fez uma inserção de resultado com sucesso: {Usuario}", usuarioorigem);

                return Created(string.Empty, "Redistribuição realizar com sucesso."); // Retornar 201 Created com mensagem
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
