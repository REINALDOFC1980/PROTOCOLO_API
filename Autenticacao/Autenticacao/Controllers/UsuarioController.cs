using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Autenticacao.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Triagem.Model;

namespace Autenticacao.Controllers
{
    [Route("api/usuario/v1")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _service;

        public UsuarioController(IUsuarioRepository services) => _service = services;


        [HttpGet("localizarusuario")]
        [CustomAuthorize("Administrador")]
        public async Task<IActionResult> LocalizarUsuario()
        {
            try
            {
                var usuario = await _service.LocalizarUsuarios();

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

        [HttpGet("localizarusuarioid/{id}")]
        [CustomAuthorize("Administrador")]
        public async Task<IActionResult> LocalizarUsuarioid(int id)
        {
            try
            {
                var usuario = await _service.LocalizarUsuarioId(id);

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

        [HttpPost("inserirusuario")]
        [CustomAuthorize("Administrador")]
        public async Task<IActionResult> InserirUsuario(UsuarioModel usuarioModel)
        {
            try
            {
                await _service.InserirUsuario(usuarioModel);
                Log.Information("Usuário fez uma inserção de resultado com sucesso: {Usuario}", usuarioModel.AutId);

                return Created(string.Empty, "Usuario inserida com sucesso."); // Retornar 201 Created com mensagem
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPut("editarusuario")]
        [CustomAuthorize("Administrador")]
        public async Task<IActionResult> EditarUsuario(UsuarioModel usuarioModel)
        {
            try
            {
                await _service.EditarUsuario(usuarioModel);
                Log.Information("Usuário fez uma alteração de resultado com sucesso: {Usuario}", usuarioModel.AutId);

                return NoContent(); // Retornar 204 No Content
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpDelete("deletarusuario/{id}")]
        [CustomAuthorize("Administrador")]
        public async Task<IActionResult> DeletarUsuario(int id)
        {
            try
            {
                await _service.ExcluirUsuarioId(id); // Chamada sem atribuição
                var response = new { Message = "Usuario deletado com sucesso." };
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
