using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datatec.DTO;
using Datatec.Infrastructure;
using Datatec.Interfaces;

namespace Datatec.Implementation
{
    public class TimeService : ITimeService
    {
        private readonly IFileService fileService;
        private readonly ILogService logService;
        private List<Periodo> _periodos;
        private Periodo _periodoAtual;

        private List<Periodo> getPeriodos()
        {
            List<Periodo> ret = new List<Periodo>();
            var section = ConfigurationManager.GetSection("MonitorSettings");

            if (section != null)
            {
                var periodos = (section as MonitorSettings).Periodos;
                foreach (var periodo in periodos)
                {
                    PeriodoSetting p = (PeriodoSetting)periodo;
                    ret.Add( new Periodo() { Nombre = p.Nombre, HoraInicio = p.HoraInicio, HoraFin = p.HoraFin, IntervaloRevision = p.IntervaloRevision ,SilencioPermitido=p.SilencioPermitido} );
                 }
            }
            return ret;

        }
        private void InitializeService()
        {
            try
            {
                string filepath=ConfigurationManager.AppSettings["LastEventFile"];
                fileService.ConfigurePath(filepath, FileMode.Truncate);
                _periodos = getPeriodos();
               
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

            for (int i = 0 ;  i < _periodos.Count && !isActiveHours ; i++)
            {
                if (Between(currentTime, _periodos[i].HoraInicio , _periodos[i].HoraFin))
                {
                    isActiveHours = true;
                    _periodoAtual = _periodos[i];
                }
              
            }
            return isActiveHours;
         
        }

        private bool Between(DateTime input, DateTime date1, DateTime date2)
        {
            return (input > date1 && input < date2);
        }

        public bool EncuentraSilencioAnormal()
        {
            bool encontrado = false;
            TimeSpan time = TimeSpanSinceLastEvent();
            if (time > _periodoAtual.SilencioPermitido)
                encontrado=true;
                            
            return encontrado;
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
