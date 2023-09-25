using Autenticacao.Model;
using Autenticacao.Services.IRepository;

using Microsoft.AspNetCore.Mvc;

namespace Autenticacao.Controllers
{
    [Route("api")]
    [ApiController]

    public class AutenticacaoController : ControllerBase
    {
       
        private readonly IAutenticacaoRepository _autenticacao;
   
        public AutenticacaoController(IConfiguration configuration, IAutenticacaoRepository autenticacaoServices)
        {           
            _autenticacao = autenticacaoServices;    
        }


        [HttpPost]
        [Route("autenticacao")]
        public async Task<ActionResult>logar([FromBody] AutenticacaoModel autenticacao)
        {
            try
            {
                var autenticacaoModel = _autenticacao.AutenticarUsuario(autenticacao.autEmail, autenticacao.autSenha);
                if (autenticacaoModel == null)
                {
                    return BadRequest(new { erro = 400, mensagem = "Usuario ou senha invalidos" });
                }
                var token = _autenticacao.GenerarToken(autenticacaoModel);
                return Ok(new { autenticacaoModel, token });
            }
            catch (Exception ex)
            {
                throw;
            }

        }



    }
}
