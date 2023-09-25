using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Data;
using Triagem.Model;
using Triagem.Service.IRepository;

namespace Triagem.Controllers
{
    [Route("api/triagem/v1")]
    [ApiController]
    public class TriagemController : ControllerBase
    {
        private readonly IProcessoServices _service;

        public TriagemController(IProcessoServices services) => _service = services;

        [HttpGet("buscarprocessoAll/{usuario}/{situacao}")]
        [CustomAuthorize("Administrador, Triagem")]
        public async Task<IActionResult> BuscarProcessoAll(string usuario, string situacao)
        {
            try
            {
                var processo = await _service.BuscarProcessoAll(usuario, situacao);

                if (processo == null)
                {
                    return BadRequest(new { erro = 400, mensagem = "Não existe dados para essa pesquisa" });
                }

                return Ok(processo);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

   
        [HttpGet("buscarquantidadeprocessos/{usuario}")]
        [CustomAuthorize("Administrador, Triagem")]
        public async Task<IActionResult> BuscarQuantidadeProcessos(string usuario)
        {
            try
            {
                var qtdprocesso = await _service.BuscarQuantidadeProcessos(usuario);
               
                if (qtdprocesso == null)
                {
                    return BadRequest(new { erro = 400, mensagem = "Não existe dados para essa pesquisa" });
                }

                return Ok(qtdprocesso);
            }
            catch (Exception ex)
            {
                throw;
            }
     
        }


        [HttpGet("buscarprocessosId/{id}")]
        [CustomAuthorize("Administrador, Triagem")]
        public async Task<IActionResult> BuscarProcessosId(int id)
        {
            try
            {
                var processo = await _service.BuscarProcessoId(id);

                if (processo == null)
                {
                    return BadRequest(new { erro = 400, mensagem = "Não existe dados para essa pesquisa" });
                }

                return Ok(processo);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet("buscardocumentosanexado/{id}")]
        [CustomAuthorize("Administrador, Triagem")]
        public async Task<IActionResult> BuscarDocumentosAnexado(int id)
        {
            try
            {
                var anexoprocesso = await _service.BuscarDocumentosAnexado(id);

                if (anexoprocesso == null)
                {
                    return BadRequest(new { erro = 400, mensagem = "Não existe dados para essa pesquisa" });
                }

                return Ok(anexoprocesso);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
      
       
        [HttpPost("inserirresultado")]
        [CustomAuthorize("Administrador, Triagem")]
        public async Task<IActionResult> InserirResultado(TriagemProcessoModel triagemProcesso)
        {
            try
            {
                await _service.InserirResultado(triagemProcesso);
                Log.Information("Usuário fez uma inserção de resultado com sucesso: {Usuario}", triagemProcesso.Tgm_Pro_Id);

                return Created(string.Empty, "Historico do processo inserida com sucesso."); // Retornar 201 Created com mensagem
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

      
        [HttpGet("buscarmotivoscancelamento")]
        [CustomAuthorize("Administrador, Triagem")]
        public async Task<IActionResult> BuscarMotivoCancelamento()
        {
            try
            {
                var motivos = await _service.BuscarMotivoCancelamento();

                if (motivos == null)
                {
                    return BadRequest(new { erro = 400, mensagem = "Não existe dados para essa pesquisa" });
                }

                return Ok(motivos);
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        [HttpPost("inserirmotivoreprovacao")]
        [CustomAuthorize("Administrador, Triagem")]
        public async Task<IActionResult> InserirMotivoReprovacao(MotivoCancelamentoModel motivoCancelamento)
        {
            try
            {
                await _service.InserirMotivoReprovacao(motivoCancelamento);
                return Created(string.Empty, "Motivo inserida com sucesso."); // Retornar 201 Created com mensagem
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    
        [HttpGet("buscamotivoscancelamentoprocesso/{id}")]
        [CustomAuthorize("Administrador, Triagem")]
        public async Task<IActionResult> BuscarMotivoCancelamentoprocesso(int id)
        {
            try
            {
                var motivos = await _service.BuscarMotivoCancelamentoProcesso(id);

                if (motivos == null)
                {
                    return BadRequest(new { erro = 400, mensagem = "Não existe dados para essa pesquisa" });
                }

                return Ok(motivos);
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        [HttpDelete("deletemotivoprocesso/{id}")]
        [CustomAuthorize("Administrador, Triagem")]
        public async Task<IActionResult> DeleteMotivoProcesso(int id)
        {
            try
            {
                await _service.DeleteMotivoProcesso(id); // Chamada sem atribuição
                var responseObj = new { Message = "Motivo deletado com sucesso." };
                return Ok(responseObj);
            }
            catch (Exception ex)
            {
                throw;
            }
        
        }
    }
}
