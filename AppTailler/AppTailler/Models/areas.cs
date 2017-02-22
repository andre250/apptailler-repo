using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTailler.Models
{
    public class areas
    {
        public int AreaCodigo { get; set; }
        public int UnidadeCodigo { get; set; }
        public int LocalCodigo { get; set; }
        public string AreaNome { get; set; }
        public int AreaStatus { get; set; }
        public ObservableCollection<ItemGroups> ListaItens { get; set; }
        public List<itens> Itens { get; set; }
    }
}
