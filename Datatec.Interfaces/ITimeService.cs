using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Interfaces
{
    public interface ITimeService
    {
        DateTime GetCurrentTime();
        bool IsActiveHours();
        TimeSpan TimeSpanSinceLastEvent();
        bool EncuentraSilencioAnormal();

        DateTime ReadLastEventTime();
        void WriteLastEventTime();
    }
}
