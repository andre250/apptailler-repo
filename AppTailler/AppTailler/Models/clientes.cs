using SQLite.Net.Attributes;

namespace AppTailler.Models
{
    public class clientes
    {
        [PrimaryKey]
        public int IdUnidade { get; set; }
        public int idPessoa { get; set; }
        public int IdEvento { get; set; }
        public string cevento { get; set; }
        public string TipoUnidade { get; set; }
        public string Unidade { get; set; }
        public string Cliente { get; set; }
        public string Logo { get; set; }
        public string Ativo { get; set; }
        public string Gestor { get; set; }
        public string Administrador { get; set; }
        public string RespNome { get; set; }
        public string RespCargo { get; set; }
        public string RespTelefone { get; set; }
        public string RespEmail { get; set; }
        public string Classificacao { get; set; }
        public string Assinatura { get; set; }
        public byte[] AssinaturaArray { get; set; }
        public int StatusAuditoria { get; set; }
    }
}
