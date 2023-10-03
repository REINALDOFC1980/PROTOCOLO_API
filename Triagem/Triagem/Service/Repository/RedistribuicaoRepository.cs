using Dapper;
using Triagem.Model;
using Triagem.Service.IRepository;
using Triagem.DBContext;

namespace Triagem.Service.Repositoty
{
    public class RedistribuicaoRepository : IRedistribuicaoRepository
    {

        private readonly DapperContext _context;

        public RedistribuicaoRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<List<RedistribuicaoModel>> BuscarProcessoUsuario()
        {
            List<RedistribuicaoModel> model = new List<RedistribuicaoModel>();

            var query = @" Select   
                                 Dist_NomeUsuario,  
                                 count(*) as DisT_Quantprocesso,  
                                 case Dist_Status   
                                      when 1 then 'Ativo'  
                                      when 0 then 'Inativo' end as Dist_Status                          
                           from  Processo join HistoricoStatusProcesso on ( Pro_Id = HisPro_Pro_Id ) and   
                                                HisPro_DataOcorrencia in (select top 1 b.HisPro_DataOcorrencia   
                                                                            from HistoricoStatusProcesso b  
                                                                            where (Pro_Id = b.HisPro_Pro_Id)  
                                                                            order by b.HisPro_DataOcorrencia desc)  
                                          join Dist_Revisor on (Pro_UsuarioRevisor = Dist_NomeUsuario)  
                           where HisPro_StsPro_Id in( 4)   
                             and Pro_Svc_Id = 1  
                        group by Dist_NomeUsuario, Dist_Status ";

            using (var connection = _context.CreateConnection())
            {
                var command = connection.Query<RedistribuicaoModel>(query);

                return command.ToList();

            }


        }

        public async Task RedistribuicaoProcesso(string usuarioorigem, string usuariodestino, int quantidade)
        {
            var Query = @"
                        UPDATE TOP (@quantidade) Processo
                        SET Pro_UsuarioRevisor = @usuariodestino
                        WHERE Pro_UsuarioRevisor = @usuarioorigem;

                        UPDATE Dist_Revisor
                        SET Dist_Qtd = Dist_Qtd - @quantidade
                        WHERE Dist_NomeUsuario = @usuarioorigem;

                        UPDATE Dist_Revisor
                        SET Dist_Qtd = Dist_Qtd + @quantidade
                        WHERE Dist_NomeUsuario = @usuariodestino;
                        ";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(Query, new
                {
                    usuarioorigem,
                    usuariodestino,
                    quantidade
                });
            }
        }

    }
}
