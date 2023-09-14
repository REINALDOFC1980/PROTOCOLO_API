using Triagem.Model;

namespace Triagem.Service.IRepository
{
    public interface IProcessoServices
    {

        Task<List<ProcessoModel>> BuscarProcessoAll(string usuario, string situacao);
        Task<List<QuantidadeProcessoModel>> BuscarQuantidadeProcessos(string usuario);
        Task<ProcessoModel> BuscarProcessoId(int id);
        Task<List<AnexoModel>> BuscarDocumentosAnexado(int id);
        Task InserirResultado(TriagemProcessoModel triagemProcesso);
        Task<List<MotivoCancelamentoModel>> BuscarMotivoCancelamento();
        Task InserirMotivoReprovacao(MotivoCancelamentoModel motivoCancelamento);
        Task<List<MotivoCancelamentoModel>> BuscarMotivoCancelamentoProcesso(int id);
        Task DeleteMotivoProcesso(int id);
    }
}
