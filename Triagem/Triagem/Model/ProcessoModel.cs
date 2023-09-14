namespace Triagem.Model
{
    public class ProcessoModel
    {
        public int Pro_Id { get; set; }
        public string? Pro_Numero { get; set; }
        public string? Pro_DataCriacao { get; set; }
        public int? Pro_Ptr_Id { get; set; }
        public int? Pro_Con_Id { get; set; }
        public int? Pro_Req_Id { get; set; }
        public int? Pro_Mut_Id { get; set; }
        public int? Pro_Vec_Id { get; set; }

        
        public string? Pro_ConfirmarProcesso { get; set; }
        public string? Pro_DT_ConfirmacaoProcesso { get; set; }
        public string? Pro_UsuarioRevisor { get; set; }
        public string? Pro_UsuarioSupervisor { get; set; }
        public int? Pro_Svc_Id { get; set; }
        public string? Pro_NumProtocoloCondutor { get; set; }
        public string? Pro_CondutorResult { get; set; }
        public int Pro_QuantidadeDias { get; set; }
        
        public MultasModel? MultasModels { get; set; }
        public VeiculoModel? VeiculoModel { get; set; }
        public CondutorModel? CondutorModel { get; set; }
        public EndCondutoModel? EndCondutoModel { get; set; }
        public RequerenteModel? RequerenteModel { get; set; }
        public EndRequerenteModel? EndRequerenteModel { get; set; }



    }



}

