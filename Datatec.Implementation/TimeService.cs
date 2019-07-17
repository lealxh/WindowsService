using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datatec.Infrastructure;
using Datatec.Interfaces;

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
            DateTime now = DateTime.Now;
            logService.Log(LogLevel.Debug, "Now: "+now.ToString());
            return now;
        }

        public bool IsActiveHours()
        {
            bool isActiveHours = false;

            DateTime currentTime = GetCurrentTime();

            for (int i = 0 ;  i < _workingHours.Count && !isActiveHours ; i++)
            {
                if (_workingHours[i].InitialTime <= currentTime && currentTime <= _workingHours[i].FinalTime)
                  isActiveHours = true;
              
            }
            return isActiveHours;
         
        }

        public void WriteLastEventTime()
        {
            DateTime currentTime = GetCurrentTime();
           //logService.Log(LogLevel.Info, String.Format("Write Last event time: " + currentTime.ToString()));
           fileService.WriteLine(currentTime.ToString());
        }

        public DateTime ReadLastEventTime()
        {
            
            String line = fileService.ReadFirstLine();
            DateTime time = DateTime.Parse(line);
            logService.Log(LogLevel.Debug, String.Format("LastEventTime: " + time.ToString()));
            return time;
        }

        public TimeSpan TimeSpanSinceLastEvent()
        {
            DateTime currentTime = GetCurrentTime();
            DateTime lastEventTime= ReadLastEventTime();
            TimeSpan ts = currentTime - lastEventTime;
            logService.Log(LogLevel.Debug, String.Format("TimeSpanSinceLastEvent: " + ts.ToString()));
            return ts;

        }

    }
}
