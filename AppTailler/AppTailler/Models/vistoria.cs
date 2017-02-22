using System.Collections.Generic;
using System.Linq;

namespace AppTailler.Models
{
    public class vistoria
    {
        public int UnidadeCodigo { get; set; }
        public int LocalCodigo { get; set; }
        public int Auditor { get; set; }
        public int TipoVistoria { get; set; }
        public int Norma { get; set; }
        public int IdAuditoriaBase { get; set; }
        public int IdEvento { get; set; }
        public string Versao { get; set; }
        public string DeviceID { get; set; }
        public string Evento { get; set; }
        public string Data { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public string RespNome { get; set; }
        public string RespTelefone { get; set; }
        public string RespEmail { get; set; }
        public string RespCargo { get; set; }
        public string Assinatura { get; set; }
        public int Cancelado { get; set; }
        public string MotivoCancelado { get; set; }
        public List<areas_sql> Areas { get; set; }

        public List<imagem> GetAreas(int idLocal)
        {
            List<imagem> listaEnvioImagens = new List<imagem>();
            using (var dados = new DataAccess())
            {
                List<auditoria> auditoriaList = dados.GetAllAuditorias(idLocal);
                var listareas = auditoriaList
                    .GroupBy(o => o.IdArea)
                    .Select(o => o.FirstOrDefault());
                List<areas_sql> ListAreas = new List<areas_sql>();
                foreach (auditoria a in listareas)
                {
                    var listitens = from d in auditoriaList
                                    where d.IdArea == a.IdArea
                                    group d by d.IdItem;

                    List<itens_sql> ListItens = new List<itens_sql>();
                    foreach (var aud in listitens)
                    {
                        auditoria item = aud.FirstOrDefault();

                        List<subitens_sql> ListSubItens = new List<subitens_sql>();
                        foreach (var audi in aud)
                        {
                            if (audi.SubCheck == 0)
                            {
                                audi.SubCheck = 3;
                            }
                            if (audi.SubMotivoBytes != null)
                            {
                                var fotoSub = new imagem()
                                {
                                    NomeImagem = audi.SubPhotoUrl + ".jpg",
                                    ArrayBytes = audi.SubMotivoBytes,
                                    Tipo = "Foto"
                                };
                                listaEnvioImagens.Add(fotoSub);
                            }
                            subitens_sql subit = new subitens_sql()
                            {
                                SubCodigo = audi.IdSubItem,
                                //SubPeso = audi.SubPeso,
                                SubCheck = audi.SubCheck,
                                SubCodigoTexto = audi.SubDescId,
                                SubDescTexto = audi.SubDescTexto,
                                SubPhotoUrl = audi.SubPhotoUrl
                            };
                            ListSubItens.Add(subit);
                        }
                        itens_sql it = new itens_sql() { ItemCodigo = item.IdItem, SubItens = ListSubItens };

                        ListItens.Add(it);
                    }

                    areas_sql ar = new areas_sql { AreaCodigo = a.IdArea, Itens = ListItens };

                    ListAreas.Add(ar);
                }
                this.Areas = ListAreas;
            }
            return listaEnvioImagens;
        }
    }
}
