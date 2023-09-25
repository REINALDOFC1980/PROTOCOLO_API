using Autenticacao.DBContext;
using Autenticacao.Model;
using Autenticacao.Services.IRepository;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Autenticacao.Services.Repository
{
    public class AutenticacaoRepository : IAutenticacaoRepository
    {
        private readonly DapperContext _context;
        private readonly  IConfiguration _config;

        public AutenticacaoRepository(DapperContext context, IConfiguration configuration) 
        {
            _context = context;
            _config = configuration;
        }

        public AutenticacaoModel AutenticarUsuario(string AutEmail, string AutSenha)
        {
           
            using (var connection = _context.CreateConnection())
            {
                var parametros = new
                {
                    AutEmail,
                    AutSenha
                };

                var model =  connection.QueryFirstOrDefault<AutenticacaoModel>("Stb_APIAutenticacao", parametros, commandType: CommandType.StoredProcedure);

                return model;
            }
            
        }

        public string GenerarToken(AutenticacaoModel autenticacao)
        {
            var tokeHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, autenticacao.autNome),
                    new Claim(ClaimTypes.Role, autenticacao.autRole),
                }),

                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key),
                                                                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokeHandler.CreateToken(tokenDescriptor);

            return tokeHandler.WriteToken(token);
        }
    }
}

