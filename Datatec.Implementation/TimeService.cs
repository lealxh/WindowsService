using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datatec.Infrastructure;
using Datatec.Services;

namespace Datatec.Implementation
{

    public class Interval
    {
        public DateTime InitialTime { get; set; }
        public DateTime FinalTime { get; set; }
    }
    public class TimeService : ITimeService
    {
        private readonly IFileService fileService;
        private readonly ILogService logService;
        private List<Interval> _workingHours;


        private void InitializeService()
        {
            try
            {

                string filepath=ConfigurationManager.AppSettings["LastEventFile"];
                fileService.ConfigurePath(filepath, FileMode.Truncate);
                string workingHoursStr = ConfigurationManager.AppSettings["WorkingHours"];
               _workingHours = new List<Interval>();
                string[] intervals = workingHoursStr.Split(',');
                foreach (string interval in intervals)
                {
                    string[] hours = interval.Split('-');
                    string date = DateTime.Now.ToShortDateString();

                    DateTime initialTime = DateTime.Parse(date + " " + hours[0]);
                    DateTime finalTime = DateTime.Parse(date + " " + hours[1]);

                    _workingHours.Add(new Interval()
                    {
                        InitialTime = initialTime,
                        FinalTime=finalTime

                    
                    });


                }
            }
            catch (ConfigurationErrorsException ex)
            {
                logService.Log(LogLevel.Error, "Error en la configuracion del servicio ");
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (Exception ex)
            {

                logService.Log(LogLevel.Error, "Error en la configuracion del servicio ");
                logService.Log(LogLevel.Error, ex.ToString());
            }
           

        }
        public TimeService(IFileService fileService,ILogService logService)
        {
            this.fileService = fileService;
            this.logService = logService;
            InitializeService();
           

        }

        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        public bool IsActiveHours()
        {
            bool isActiveHours = false;
       
            foreach (Interval interval in _workingHours)
             if (interval.InitialTime <= GetCurrentTime() && GetCurrentTime() <= interval.FinalTime)
                    isActiveHours = true;

            return isActiveHours;
         
        }

        public void WriteLastEventTime()
        {
            fileService.WriteLine(DateTime.Now.ToString());
        }

        public DateTime ReadLastEventTime()
        {
            DateTime time = DateTime.Now;
            String line = fileService.ReadFirstLine();
            DateTime.TryParse(line, out time);
            return time;
        }

        public TimeSpan TimeSpanSinceLastEvent()
        {
            TimeSpan ts = GetCurrentTime() - ReadLastEventTime();
            return ts;

        }

    }
}
