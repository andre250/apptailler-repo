using SQLite.Net.Attributes;

namespace AppTailler.Models
{
    public class subitens
    {
        [PrimaryKey, AutoIncrement]
        public int SubCodigo { get; set; }
        public string IdAuditoria { get; set; }
        public int ItemCodigo { get; set; }
        public string SubDescricao { get; set; }
        public float SubPeso { get; set; }
        //SubCheck -- Null = 0 / Confirm = 1 / Unconfirm = 2 / N.A = 3
        public int SubCheck { get; set; }
        //SubChecked -- Uncheck = 0 / Check = 1 /
        public int SubChecked { get; set; }
        public string SubCheckConfirmSource { get; set; }
        public string SubCheckUnconfirmSource { get; set; }
        public string SubCheckNaSource { get; set; }
        public int SubCodigoTexto { get; set; }
        public string SubDescTexto { get; set; }
        public string SubPhotoUrl { get; set; }
        public byte[] SubMotivoBytes { get; set; }
        public override string ToString()
        {
            return string.Format(SubDescricao);
        }
    }


}
