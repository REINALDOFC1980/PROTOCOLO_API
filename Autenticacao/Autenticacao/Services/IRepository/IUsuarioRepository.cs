using Autenticacao.Model;
using Triagem.Model;

namespace Autenticacao.Services.IRepository
{
    public interface IUsuarioRepository
    {
        public Task<List<UsuarioModel>>  LocalizarUsuarios();
        public Task<UsuarioModel> LocalizarUsuarioId(int id);        
        public Task InserirUsuario(UsuarioModel usuarioModel);
        public Task EditarUsuario(UsuarioModel usuarioModel);
        public Task ExcluirUsuarioId(int id);

    }
}
