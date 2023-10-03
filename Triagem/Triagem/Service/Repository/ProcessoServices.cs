using Dapper;
using Triagem.DBContext;
using Triagem.Model;
using Triagem.Service.IRepository;

namespace Triagem.Service.Repositoty
{
    public class ProcessoServices : IProcessoServices
    {
        private readonly DapperContext _context;

        public ProcessoServices(DapperContext context) => _context = context;

        public Task<List<AnexoModel>> BuscarDocumentosAnexado(int id)
        {

            List<AnexoModel> model = new List<AnexoModel>();
            
                var query = @" 
                        select Prtdoc_prt_numero, 
	                           Prtdoc_imagem, 
	                           Doc_nome 
                          from IntegrarProcessoProtocolo inner join 
                               GR_Protocolo_BASalvador.dbo.Protocolo_Documento_Imagem on (Ipp_NumeroProtocolo = PrtDoc_Prt_Numero) inner join [GR_Protocolo_BASalvador].dbo.Documento on (PRTDOC_DOC_ID = DOC_ID)
                         where PRTDOC_PRT_SETOR = 92 and  
                               Ipp_Pro_Id = @id ";

                using (var connection = _context.CreateConnection())
                {
                    var parametros = new { id };
                    var command = connection.Query<AnexoModel>(query, parametros);

                    // Convert byte[] to base64-encoded string
                    foreach (var item in command)
                    {
                        if (item.Prtdoc_imagem != null)
                        {
                            item.Base64Image = Convert.ToBase64String(item.Prtdoc_imagem);
                        }

                    }

                    return Task.FromResult(command.ToList()); ; // Encapsula a lista em uma Task
                }
            }        

        public async Task<List<ProcessoModel>> BuscarProcessoAll(string usuario, string situacao)
        {
            List<ProcessoModel> model = new List<ProcessoModel>();
          
                var query = @" 
                            SELECT
                             Pro_Id                                 
                            ,Pro_Numero           
                            ,Convert(varchar(10), Pro_DataCriacao, 103) as Pro_DataCriacao                                   
                            ,DATEDIFF(day,Pro_DT_ConfirmacaoProcesso,getdate()) as Pro_QuantidadeDias                         
                            
                            ,Mut_Id
                            ,Mut_AIT
                        
                            ,Vec_Id
                            ,Vec_Placa 
                
                            ,Con_Id
	                        ,Upper(Con_Nome) as Con_Nome                                                                                                                                                                                                                                                              
	                        ,Con_Email 

                            ,Req_Id
	                        ,Upper(Req_Nome) as Req_Nome           	                               
	                        ,Req_Email

                        FROM Processo
                             INNER JOIN Requerente ON (Pro_Req_Id = Req_Id)
                             INNER JOIN Multa ON (Pro_Mut_Id = Mut_Id)
                             INNER JOIN Veiculo ON (Pro_Vec_Id = Vec_Id)
                             INNER JOIN HistoricoStatusProcesso HisA ON (Pro_Id = HisPro_Pro_Id)
                             INNER JOIN StatusProcesso sp ON (HisPro_StsPro_Id = StsPro_id)
		                     INNER JOIN Condutor ON (Pro_Con_Id = Con_id)

                       WHERE HisPro_DataOcorrencia IN ( SELECT MAX(HisPro_DataOcorrencia) 
                                                          FROM HistoricoStatusProcesso HisB 
                                                         WHERE HisA.HisPro_Pro_Id = HisB.HisPro_Pro_Id
                                                        )
                            AND sp.StsPro_Descricao LIKE CASE WHEN @situacao = '' THEN 'Aguardando Triagem' ELSE @situacao END
                            AND Pro_UsuarioRevisor = @usuario
                            AND Pro_Svc_Id IN (1)
                            ORDER BY Processo.Pro_Id desc";


                using (var connection = _context.CreateConnection())
                {
                    var parametros = new { usuario, situacao };

                    var command = connection.Query<ProcessoModel, MultasModel, VeiculoModel, CondutorModel, RequerenteModel, ProcessoModel>(
                        query,
                        (processo, MultasModel, VeiculoModel, CondutorModel, RequerenteModel) =>
                        {
                            processo.MultasModels = MultasModel;
                            processo.VeiculoModel = VeiculoModel;
                            processo.CondutorModel = CondutorModel;
                            processo.RequerenteModel = RequerenteModel;
                            return processo;
                        },
                        parametros,
                        splitOn: "Mut_Id, Vec_Id, Con_Id, Req_Id"
                     );


                    return command.ToList();
                }

            }          

        public async Task<ProcessoModel> BuscarProcessoId(int id)
        {
            ProcessoModel model = new ProcessoModel();
    
                var query = @" 
                             SELECT       
                             Pro_Id                                 
	                        ,Pro_Numero           
	                        ,FORMAT(Pro_DataCriacao, 'dd/MM/yyyy HH:mm:ss')  as Pro_DataCriacao          
	                        ,DATEDIFF(day,Pro_DT_ConfirmacaoProcesso, getdate()) as Pro_QuantidadeDias

	                        ,Mut_Id 
	                        ,Mut_AIT
	                        ,Mut_CodigoInfracao 
	                        ,Mut_DescricaoInfracao 

	                        ,FORMAT(Mut_DataInfracao, 'dd/MM/yyyy') AS Mut_DataInfracao
	
	                        ,Vec_Id               
	                        ,Vec_Placa 
	                        ,Vec_RENAVAN 
	                        ,Vec_Ano  
	                        ,Vec_Marca

	                        ,Con_Id                                  
	                        ,Upper(Con_Nome) as Con_Nome                                                                                                                                                                                                                                                         
	                        ,Con_CPF                                            
	                        ,Con_Telefone                                                                             
	                        ,Con_RegistroCNH      
	                        ,Convert(varchar(10),Con_CNHValidade,103) as Con_CNHValidade        
	                        ,Con_RG               
	                        ,Con_Email  
	                        ,Con_FotoCNH                    
	                        ,Con_Estrangeiro 
	                        ,Con_UF_CNH
                           

                            ,Con_End.EndPro_Id            as EndCon_Id  
	                        ,Con_End.EndPro_CEP           as EndCon_CEP            
	                        ,Con_End.EndPro_Logradouro    as endCon_Logradouro      
	                        ,Con_End.EndPro_Complemento   as EndCon_Complemento     
	                        ,Con_End.EndPro_Numero        as EndCon_Numero          
	                        ,Con_End.EndPro_Bairro        as EndCon_Bairro          
	                        ,Con_End.EndPro_Cidade        as EndCon_Cidade          	
	                        ,Con_End.EndPro_Estado        as EndCon_Estado          	
	                        ,Con_End.EndPro_Pais          as EndCon_Pais         

	                        ,Req_Id              
	                        ,Upper(Req_Nome) as Req_Nome           
	                        ,Req_CPF_CNPJ        
	                        ,Req_Telefone                 
	                        ,Req_RG              
	                        ,Req_Email                                   
                            ,Upper(Req_NomeFantasia) 		as Req_NomeFantasia  	 

	                        ,Req_End.EndPro_Id              as EndReq_Id                    
	                        ,Req_End.EndPro_CEP			    as EndReq_CEP		   
	                        ,Req_End.EndPro_Logradouro      as EndReq_Logradouro                                                                         
	                        ,Req_End.EndPro_Complemento     as EndReq_Complemento                                                                                                                                                                            
	                        ,Req_End.EndPro_Numero          as EndReq_Numero                           
	                        ,Req_End.EndPro_Bairro          as EndReq_Bairro                                                                             
	                        ,Req_End.EndPro_Cidade          as EndReq_Cidade                                                                             
	                        ,Req_End.EndPro_Estado          as EndReq_Estado                                                                             
	                        ,Req_End.EndPro_Pais			as EndReq_Pais	

                             FROM  Processo
                                   INNER JOIN Requerente ON (Pro_Req_Id = Req_Id)
                                   INNER JOIN Multa ON (Pro_Mut_Id = Mut_Id)
                                   INNER JOIN Veiculo ON (Pro_Vec_Id = Vec_Id)
	                               INNER JOIN Condutor ON (Pro_Con_Id = Con_id)  
	                               INNER JOIN EnderecoProcesso AS Con_End on(Con_EndPro_Id = Con_End.EndPro_Id)
	                               INNER JOIN EnderecoProcesso AS Req_End on(Req_EndPro_Id = Req_End.EndPro_Id)                     
                             where pro_id = @id ";


                using (var connection = _context.CreateConnection())
                {
                    var parametros = new { id };

                    var command = connection.Query<ProcessoModel, MultasModel, VeiculoModel, CondutorModel, EndCondutoModel, RequerenteModel, EndRequerenteModel, ProcessoModel>(
                        query,
                        (processo, MultasModel, VeiculoModel, CondutorModel, EndCondutoModel, RequerenteModel, EndRequerenteModel) =>
                         {
                             processo.MultasModels = MultasModel;
                             processo.VeiculoModel = VeiculoModel;
                             processo.CondutorModel = CondutorModel;
                             processo.EndCondutoModel = EndCondutoModel;
                             processo.RequerenteModel = RequerenteModel;
                             processo.EndRequerenteModel = EndRequerenteModel;

                             return processo;
                         },
                        parametros,
                        splitOn: "Mut_Id, Vec_Id, Con_Id, EndCon_Id,Req_Id, EndReq_Id"

                    ); ;

                    var result = command.SingleOrDefault(); // Obtenha o único item da coleção

                    return result;
                }

            }

        public Task<List<QuantidadeProcessoModel>> BuscarQuantidadeProcessos(string usuario)
        {
            List<QuantidadeProcessoModel> model = new List<QuantidadeProcessoModel>();
           
                var query = @" 
                    WITH Categorias AS (
                        SELECT
                            Processo.Pro_Id,
                            CASE
                                WHEN DATEDIFF(DAY, HisA.HisPro_DataOcorrencia, GETDATE()) BETWEEN 0 AND 2 THEN 'Novo'
                                WHEN DATEDIFF(DAY, HisA.HisPro_DataOcorrencia, GETDATE()) BETWEEN 3 AND 4 THEN 'Recente'
                                WHEN DATEDIFF(DAY, HisA.HisPro_DataOcorrencia, GETDATE()) > 5 THEN 'Atrasado'
                                ELSE 'Outro'
                            END AS Tipo
                        FROM
                            Processo
                            INNER JOIN Requerente ON (Processo.Pro_Req_Id = Requerente.Req_Id)
                            INNER JOIN Multa ON (Processo.Pro_Mut_Id = Multa.Mut_Id)
                            INNER JOIN Veiculo ON (Processo.Pro_Vec_Id = Veiculo.Vec_Id)
                            INNER JOIN HistoricoStatusProcesso HisA ON (Processo.Pro_Id = HisA.HisPro_Pro_Id)
                            INNER JOIN StatusProcesso sp ON (HisA.HisPro_StsPro_Id = sp.StsPro_id)
                            INNER JOIN Condutor ON (Processo.Pro_Con_Id = Condutor.Con_id)
                        WHERE
                            HisA.HisPro_DataOcorrencia IN (
                                SELECT MAX(HisPro_DataOcorrencia) 
                                FROM HistoricoStatusProcesso HisB 
                                WHERE HisA.HisPro_Pro_Id = HisB.HisPro_Pro_Id
                            )
                            AND HisA.HisPro_StsPro_Id = 4
                            AND Processo.Pro_UsuarioRevisor =  @usuario
                            AND Processo.Pro_Svc_Id IN (1)
                    )
                    SELECT
                        SUM(CASE WHEN C.Tipo = 'Novo' THEN 1 ELSE 0 END) AS Novo,
                        SUM(CASE WHEN C.Tipo = 'Recente' THEN 1 ELSE 0 END) AS Recente,
                        SUM(CASE WHEN C.Tipo = 'Atrasado' THEN 1 ELSE 0 END) AS Atrasado,
                        SUM(CASE WHEN C.Tipo NOT IN ('Novo', 'Recente', 'Atrasado') THEN 1 ELSE 0 END) AS Outro,
                        COUNT(*) AS Total
                    FROM Categorias C";

                using (var connection = _context.CreateConnection())
                {
                    var parametros = new { usuario };
                    var command = connection.Query<QuantidadeProcessoModel>(query, parametros);
                    return Task.FromResult(command.ToList()); // Encapsula a lista em uma Task
                }
        }

        public async Task InserirResultado(TriagemProcessoModel triagemProcesso)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction()) // Start the transaction
                {

                    var insertQueryHistorico = @"
                              INSERT INTO HistoricoStatusProcesso 
                                    (HisPro_DataOcorrencia,
                                     HisPro_Pro_Id,
                                     HisPro_StsPro_Id)
                              SELECT GETDATE(),
                                     @pro_Id,
                                     case when @resul_id = 1 then 7 else 6 end
                               WHERE @Pro_Id NOT IN (SELECT HisPro_Pro_Id FROM HistoricoStatusProcesso WHERE HisPro_Pro_Id = @pro_Id AND HisPro_StsPro_Id = 7);";

                    var insertQueryTriagem = @"
                              INSERT INTO Triagem
                                    (Tgm_UsuarioRevisor,
                                     Tgm_ResultRevisor,
                                     Tgm_DataTriagem,
                                     Tgm_IntegradoSIP,
                                     Tgm_Pro_Id,
                                     Tgm_Mot_Id)
                               SELECT @UsuarioOperador,
	 		                          @resul_id,
			                          GETDATE(),
			                          @resul_id,
			                          @pro_Id,
			                          MotTgm_Temp_CANC_ID
		                         FROM Processo LEFT JOIN MotivosTriagem_Temporaria ON (pro_id = MotTgm_Temp_Pro_Id) 
                                WHERE pro_id = @pro_id";
                    //inserção do historico status
                    await connection.ExecuteAsync(insertQueryHistorico, new
                    {
                        pro_id = triagemProcesso.Tgm_Pro_Id,
                        resul_id = triagemProcesso.Tgm_resul_id,
                    }, transaction: transaction);

                    //inserção da triagem
                    await connection.ExecuteAsync(insertQueryTriagem, new
                    {
                        pro_id = triagemProcesso.Tgm_Pro_Id,
                        resul_id = triagemProcesso.Tgm_resul_id,
                        mot_id = triagemProcesso.Tgm_Mot_Id,
                        UsuarioOperador = triagemProcesso.Tgm_Usuario_Operador
                    }, transaction: transaction);


                    transaction.Commit(); // Commit the transaction if everything is successful
                }
            }
        }

        public async Task<List<MotivoCancelamentoModel>> BuscarMotivoCancelamento()
        {

            List<MotivoCancelamentoModel> model = new List<MotivoCancelamentoModel>();
          
            var query = @"Select Canc_Id, Canc_Nome from GR_Protocolo_BASalvador.dbo.MotivoCancelamento order by Canc_Nome ";

            using (var connection = _context.CreateConnection())
            {
                var command = connection.Query<MotivoCancelamentoModel>(query);


                return command.ToList();  // Encapsula a lista em uma Task
            }
           
        }
      
        public async Task InserirMotivoReprovacao(MotivoCancelamentoModel motivoCancelamento)
        {
                   
            var insertQueryMotivo = @"
                                INSERT INTO MotivosTriagem_Temporaria 
                                    (MotTgm_Temp_Pro_Id, 
                                        MotTgm_Temp_Canc_Id, 
                                        MotTgm_Temp_UsuarioMatrix)
                                 SELECT @Canc_Pro_id, 
                                        @Canc_Id, 
                                        @Canc_Usuario
                                WHERE NOT EXISTS (
                                    SELECT 1 FROM MotivosTriagem_Temporaria 
                                    WHERE MotTgm_Temp_Pro_Id = @Canc_Pro_id AND 
                                            MotTgm_Temp_CANC_ID = @Canc_Id
                                );";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(insertQueryMotivo, new
                {
                    Canc_Pro_id = motivoCancelamento.Canc_Pro_id,
                    Canc_Id = motivoCancelamento.Canc_Id,
                    Canc_Usuario = motivoCancelamento.Canc_Usuario
                });
            }



        }

        public async Task<List<MotivoCancelamentoModel>> BuscarMotivoCancelamentoProcesso( int id)
        {

            List<MotivoCancelamentoModel> model = new List<MotivoCancelamentoModel>();
           
                var query = @"select Canc_Id, CANC_NOME,MotTgm_Temp_Pro_Id as Canc_Pro_id
                                from MotivosTriagem_Temporaria 
                          inner join GR_Protocolo_BASalvador.dbo.MotivoCancelamento on (MotTgm_Temp_CANC_ID = CANC_ID)
                               where MotTgm_Temp_Pro_Id = @id
                                     order by Canc_Nome";

                using (var connection = _context.CreateConnection())
                {
                    var parametros = new { id };
                    var command = connection.Query<MotivoCancelamentoModel>(query , parametros);

                    return command.ToList();  //Encapsula a lista em uma Task
                }
          
        }

        public async Task DeleteMotivoProcesso(int id)
        {
            var DeleteQueryMotivo = @"
                                       Delete From MotivosTriagem_Temporaria where MotTgm_Temp_CANC_ID = @id    
                                    ";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(DeleteQueryMotivo, new{id});
            }
        }
    }
}





