
using SQLite.Net.Interop;

namespace AppTailler
{
    public interface IConfig
    {
        string DiretorioDB { get; }
        ISQLitePlatform Plataforma { get; }
    }
}
