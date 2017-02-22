using System.Collections.Generic;
using SQLite.Net.Attributes;

namespace AppTailler.Models
{
    public class auditoria
    {
        [PrimaryKey, Indexed]
        public string IdAuditoria { get; set; }
        [Indexed]
        public int IdUnidade { get; set; }
        [Indexed]
        public int IdLocal { get; set; }
        [Indexed]
        public int IdArea { get; set; }
        [Indexed]
        public string DescArea { get; set; }
        [Indexed]
        public int StatusArea { get; set; }
        [Indexed]
        public int IdItem { get; set; }
        [Indexed]
        public string DescItem { get; set; }
        [Indexed]
        public int StatusItem { get; set; }
        [Indexed]
        public int IdSubItem { get; set; }
        [Indexed]
        public string DescSubItem { get; set; }
        [Indexed]
        public int StatusSubItem { get; set; }
        [Indexed]
        public float SubPeso { get; set; }
        [Indexed]
        public int SubCheck { get; set; }
        [Indexed]
        public int SubChecked { get; set; }
        [Indexed]
        public string SubCheckConfirmSource { get; set; }
        [Indexed]
        public string SubCheckUnconfirmSource { get; set; }
        [Indexed]
        public string SubCheckNaSource { get; set; }
        [Indexed]
        public int SubDescId { get; set; }
        [Indexed]
        public string SubDescTexto { get; set; }
        [Indexed]
        public string SubPhotoUrl { get; set; }
        [Indexed]
        public byte[] SubMotivoBytes { get; set; }
    }
}
