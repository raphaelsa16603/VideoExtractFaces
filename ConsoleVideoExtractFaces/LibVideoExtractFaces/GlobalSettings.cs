using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibVideoExtractFaces
{
    public class GlobalSettings
    {
        private static readonly Lazy<GlobalSettings> _instance = new Lazy<GlobalSettings>(() => new GlobalSettings());

        public static GlobalSettings Instance => _instance.Value;

        public string SomeGlobalSetting { get; set; }

        private GlobalSettings()
        {
            // Configurações globais que podem ser utilizadas em toda a aplicação.
        }
    }
}
