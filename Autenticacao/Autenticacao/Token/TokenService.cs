using Autenticacao.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Autenticacao.Token
{
    public  class TokenService
    {
        private  readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerarToken(AutenticacaoModel autenticacao)
        {

            var tokeHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, autenticacao.AutNome),
                    new Claim(ClaimTypes.Role, autenticacao.AutRole),
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
