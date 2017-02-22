using System.Collections.ObjectModel;

namespace AppTailler.Models
{
    public class SubItems
    {
        public int SubCodigo { get; set; }
        public string IdAuditoria { get; set; }
        public int ItemCodigo { get; set; }
        public string SubDescricao { get; set; }
        public float SubPeso { get; set; }
        public int SubCheck { get; set; }
        public int SubChecked { get; set; }
        public string SubCheckConfirmSource { get; set; }
        public string SubCheckUnconfirmSource { get; set; }
        public string SubCheckNaSource { get; set; }
        public int SubCodigoTexto { get; set; }
        public string SubDescTexto { get; set; }
        public string SubPhotoUrl { get; set; }
        public byte[] SubMotivoBytes { get; set; }
        public SubItems()
        {
        }
    }
    public class ItemGroups:ObservableCollection<SubItems>
    {
        public int ItemCodigo { get; set; }
        public string ItemDescricao { get; set; }

    }
}
