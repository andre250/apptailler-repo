
using SQLite.Net.Attributes;
using System.Collections.Generic;

namespace AppTailler.Models
{
    public class locais
    {
        [PrimaryKey]
        public int IdLocal { get; set; }
        [Indexed]
        public int IdUnidade { get; set; }
        [Indexed]
        public int IdAuditoria { get; set; }
        public int IdEvento { get; set; }
        public int TipoVistoria { get; set; }
        public int IdNorma { get; set; }
        public int IdAuditoriaBase { get; set; }
        public string Nome { get; set; }
        public string Referencia { get; set; }
        public string Ativo { get; set; }
        public int StatusAuditoria { get; set; }
        public int StatusAuditoriaFila { get; set; }
        public int Cancelado { get; set; }
        public string RespNome { get; set; }
        public string RespEmail { get; set; }
        public string RespCargo { get; set; }
        public string RespTelefone { get; set; }
        public string Assinatura { get; set; }
        public string cevento { get; set; }
        public string ultimaNota { get; set; }
        public byte[] AssinaturaArray { get; set; }
        public string DtUltimaAuditoria { get; set; }
        [Ignore]
        public List<areas> auditoria { get; set; }
    }
}
