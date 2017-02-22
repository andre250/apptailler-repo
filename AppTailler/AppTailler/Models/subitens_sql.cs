using SQLite.Net.Attributes;

namespace AppTailler.Models
{
    public class subitens_sql
    {
        public int SubCodigo { get; set; }
        public float SubPeso { get; set; }
        //SubCheck -- Null = 0 / Confirm = 1 / Unconfirm = 2 / N.A = 3
        public int SubCheck { get; set; }
        //SubChecked -- Uncheck = 0 / Check = 1 /
        public int SubCodigoTexto { get; set; }
        public string SubDescTexto { get; set; }
        public string SubPhotoUrl { get; set; }
    }


}
