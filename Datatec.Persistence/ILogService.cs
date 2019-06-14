using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Persistence
{
    public interface ILogService
    {
        void Log(LogLevel LogLevel, object Message);
    }
}
