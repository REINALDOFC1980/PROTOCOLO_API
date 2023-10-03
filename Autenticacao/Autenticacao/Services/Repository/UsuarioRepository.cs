using Autenticacao.DBContext;
using Autenticacao.Model;
using Autenticacao.Services.IRepository;
using Dapper;
using Triagem.Model;

namespace Autenticacao.Services.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DapperContext _context;
        private readonly IConfiguration _config;


        public UsuarioRepository(DapperContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }

        public async Task<List<UsuarioModel>> LocalizarUsuarios()
        {
            var query = @"select
                         AutId       
                        ,AutNome                                                                                              
                        ,AutEmail                                                                                                                                                                                                                                                                                                                                                     
                        ,AutRole                        
                        ,AutCadastro
                   from ApiAutenticacao";

            using(var connection = _context.CreateConnection()) {

                var command = connection.Query<UsuarioModel>(query);

                return command.ToList();             
            }
        }

        public async Task<UsuarioModel> LocalizarUsuarioId(int id)
        {
            var query = @"select
                         AutId       
                        ,AutNome                                                                                              
                        ,AutEmail                                                                                                                                                                                                                                                                                                                                                     
                        ,AutRole                        
                        ,AutCadastro
                   from ApiAutenticacao where AutId = @id";

            using (var connection = _context.CreateConnection())
            {
                var parametros = new { id };

                var command =  connection.Query<UsuarioModel>(query, parametros);

                return command.SingleOrDefault(); 
            }
        }
     
        public async Task EditarUsuario(UsuarioModel usuarioModel)
        {
            var insertQueryUsuario = @"
                     Update APIAutenticacao
                        Set AutNome = @AutNome  
	                       ,AutEmail = @AutEmail   
	                       ,AutSenha = HashBytes('MD5', Convert(varchar,@AutSenha))      
                           ,AutRole  = @AutRole    
                      Where AutId = @AutId"; // Use GETDATE() para obter a data atual

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(insertQueryUsuario, new
                {
                    AutId = usuarioModel.AutId,
                    AutNome = usuarioModel.AutNome,
                    AutEmail = usuarioModel.AutEmail,
                    AutSenha = usuarioModel.AutSenha,
                    AutRole = usuarioModel.AutRole,
                });
            }
        }

        public async Task ExcluirUsuarioId(int id)
        {
            var DeleteQueryUsuario = @"
                                       Delete From ApiAutenticacao where AutId = @id    
                                    ";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(DeleteQueryUsuario, new { id });
            }
        }

        public async Task InserirUsuario(UsuarioModel usuarioModel)
        {

            var insertQueryUsuario = @"
                            INSERT INTO APIAutenticacao
                            (      
                                AutNome      
                               ,AutEmail     
                               ,AutSenha     
                               ,AutRole      
                               ,AutCadastro)
                            VALUES
                            ( 
                                @AutNome    
                               ,@AutEmail   
                               ,HashBytes('MD5', Convert(varchar,@AutSenha))    
                               ,@AutRole    
                               ,GETDATE())"; // Use GETDATE() para obter a data atual

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(insertQueryUsuario, new
                {
                    AutNome = usuarioModel.AutNome,
                    AutEmail = usuarioModel.AutEmail,
                    AutSenha = usuarioModel.AutSenha,
                    AutRole = usuarioModel.AutRole,
                });
            }


        }

      
    }
}
