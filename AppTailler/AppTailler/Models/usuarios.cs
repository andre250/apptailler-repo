using System;
using SQLite.Net.Attributes;

namespace AppTailler.Models
{
    public class usuarios
    {
        [PrimaryKey, AutoIncrement, Indexed]
        public int idUsuario { get; set; }
        public string Nome { get; set; }
        public string Matricula { get; set; }
        public string TipoPessoa { get; set; }
        public string Funcao { get; set; }
        public string Grupo { get; set; }
        public string Foto { get; set; }
        public string Token { get; set; }
        [Indexed]
        public int IdPessoa { get; set; }
        public int usuStatus { get; set; }
        public string usuUsuario { get; set; }
        public string usuSenha { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", idUsuario);
        }
    }
}
