using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTailler.Models
{
    public class OS
    {
        public int IdUnidade { get; set; }
        public int IdEvento { get; set; }
        public string CEvento { get; set; }
        public int IdAuditor { get; set; }
        public string Responsavel { get; set; }
        public string ResponsavelEmail { get; set; }
        public string ResponsavelTelefone { get; set; }
        public string ResponsavelCargo { get; set; }
        public string Assinatura { get; set; }
        public string Classificacao { get; set; }
        public string Versao { get; set; }
        public string DeviceID { get; set; }
    }
}
