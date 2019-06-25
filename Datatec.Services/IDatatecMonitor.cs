using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Services
{
    public interface IDatatecMonitor
    {

        void Start();

        bool isDatatecDown();

        void Stop();
    }
}
