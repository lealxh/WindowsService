using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Infrastructure
{

    public class MonitorSettings : ConfigurationSection
    {
        [ConfigurationProperty("Periodos")]
        public PeriodosCollection Periodos
        {
            get { return ((PeriodosCollection)(base["Periodos"])); }
            set { base["Periodos"] = value; }
        }
    }
}
