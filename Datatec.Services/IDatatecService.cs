using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Services
{
    public interface IDatatecService
    {
        void Start();
        String ReadInput();

        void WriteOutPut(string lastLine);

        void Stop();
    }
}
