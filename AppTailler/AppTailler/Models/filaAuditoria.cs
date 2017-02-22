using SQLite.Net.Attributes;
using System.Linq;
using System.Collections.Generic;

namespace AppTailler.Models
{
    public class filaAuditoria
    {
        [PrimaryKey, AutoIncrement]
        public int IdPessoa { get; set; }
        public int IdUnidade { get; set; }
        public int IdLocal { get; set; }
        [Ignore]
        public List<locais> listaLocal { get; set; }
        public string Classificacao { get; set; }
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public string Rg { get; set; }
        public int TotalFila { get; set; }
        public int RestoFila { get; set; }
        public List<locais> AtualizarLocaisNaoConcluidos()
        {
            List<locais> listaLocal;
            using (var dados = new DataAccess())
            {
                listaLocal = dados.GetAllLocais(IdUnidade);
                listaLocal = listaLocal.Where(p => p.StatusAuditoriaFila == 0 && p.Cancelado != 1).ToList();
            }
            return listaLocal;
        }
        public int CountLocais()
        {
            return listaLocal.Count;
        }
        public int CountLocaisEnviados()
        {
            return listaLocal.Count(p => p.StatusAuditoriaFila == 1);
        }
        public int CountLocaisRestantes()
        {
            return listaLocal.Count(p => p.StatusAuditoriaFila == 0);
        }
        public string ListaLocaisSemAssinatura()
        {
            var lista = listaLocal.Where(p => p.Assinatura == null && p.Cancelado != 1).ToList();
            var lista_nomes = lista.Select(x => x.Nome).ToArray();
            var texto = "";
            if (lista_nomes == null || lista_nomes.Length == 0)
            {
                texto = "sucesso";
                return texto;
            }else
            {
                foreach (string nome in lista_nomes)
                {
                    texto = texto + "\r\n" + nome;
                }
                return texto;
            }
        }
    }
}
