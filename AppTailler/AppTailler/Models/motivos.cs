using SQLite.Net.Attributes;

namespace AppTailler.Models
{
    public class motivos
    {
        [PrimaryKey]
        public int IdTextoVistoria { get; set; }
        [Indexed]
        public int idSubItem { get; set; }
        public int txtNumTexto { get; set; }
        public string txtTexto { get; set; }
    }
}
