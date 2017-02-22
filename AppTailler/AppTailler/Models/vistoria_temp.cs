using System;
using SQLite.Net.Attributes;

namespace AppTailler.Models
{
    public class vistoria_temp
    {
        [PrimaryKey]
        public int LocalCodigo { get; set; }
        [Indexed]
        public int UnidadeCodigo { get; set; }
        [Indexed]
        public int Auditor { get; set; }
        public int IdEvento { get; set; }
        public int Cancelado { get; set; }
        public string Data { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public string RespNome { get; set; }
        public string RespTelefone { get; set; }
        public string RespEmail { get; set; }
        public string RespCargo { get; set; }
        public string Assinatura { get; set; }
    }
}
