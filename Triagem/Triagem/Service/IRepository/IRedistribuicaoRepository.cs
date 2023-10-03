using Triagem.Model;

namespace Triagem.Service.IRepository
{
    public interface IRedistribuicaoRepository
    {

        Task<List<RedistribuicaoModel>> BuscarProcessoUsuario();

        Task RedistribuicaoProcesso(string usuarioorigem, string usuariodestino, int quantidade);
    }
}
