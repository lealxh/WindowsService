using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datatec.DTO;

namespace Datatec.Infrastructure
{
    public interface IDatabaseService
    {
        void ExecuteQuery(string query, IEnumerable<Object> Params);
        IEnumerable<Object> CreateParameters(PuntaDolarDTO punta);
    }
}
