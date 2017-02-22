using SQLite.Net.Attributes;
namespace AppTailler.Models
{
    public class settings
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int ID { get; set; }
        public bool isPersist { get; set; }
        public string lastLogin_user { get; set; }
        [Indexed]
        public int lastLogin_userId { get; set; }
        public string lastLogin { get; set; }
        public int tempIdUsuario { get; set; }
        [Indexed]
        public int tempIdUnidade { get; set; }
        [Indexed]
        public int tempIdLocal { get; set; }
        public bool modoOffline { get; set; }
        public string horaInicio { get; set; }
        public string horaFim { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", ID, isPersist, lastLogin_user, lastLogin_userId); 
        }
    }

}
