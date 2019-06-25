using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datatec.Services;

namespace Datatec.Implementation
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        public bool IsWorkingHours()
        {
            bool isWorkingHours = false;
            if (GetCurrentTime().Hour > 8 && GetCurrentTime().Hour < 20)
                isWorkingHours = true;

            return isWorkingHours;
        }
    }
}
