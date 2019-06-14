using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Persistence
{
    public interface IDatabaseService
    {
        void ExecuteQuery(string query, IEnumerable<Object> Params);
        IEnumerable<Object> CreateParameters(string data);
    }
}
