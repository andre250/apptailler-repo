using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTailler.Models
{
    public class itens
    {
        public int ItemCodigo { get; set; }
        public string ItemDescricao { get; set; }
        public List<subitens> SubItens { get; set; }
    }
}
