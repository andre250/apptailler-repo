using System.Collections.Generic;

namespace AppTailler.Models
{
    public class itens_sql
    {
        public int ItemCodigo { get; set; }
        public List<subitens_sql> SubItens { get; set; }
    }
}
