using Autenticacao.Model;

namespace Autenticacao.Services.IRepository
{
    public interface IAutenticacaoRepository
    {
      public AutenticacaoModel AutenticarUsuario(string AutEmail, string AutSenha);
      public string GenerarToken(AutenticacaoModel autenticacao);
    }
}
