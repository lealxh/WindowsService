using Datatec.DTO;
using Datatec.Infrastructure;
using Datatec.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Datatec.Implementation
{
    public class DatatecMonitor : IDatatecMonitor
    {
        
        
        private readonly INotificationService notificationService;
        private readonly ILogService logService;
        private readonly ITimeService timeService;
        private Status _status;
        private ServiceController _datatecServiceController;
        private string _datatecServerName;
        private string _datatecServiceName;
     
        private List<Tarea> _tareas;
        private Tarea tareaInicializadora;


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
                        ret.Add(new Periodo() { Nombre = p.Nombre, HoraInicio = p.HoraInicio, HoraFin = p.HoraFin, IntervaloRevision = p.IntervaloRevision, SilencioPermitido = p.SilencioPermitido });
                    }
                }

           return ret;

        }

        private bool InitializeService()
        {
            try
            {

                _datatecServerName = ConfigurationManager.AppSettings["DatatecServerName"];
                _datatecServiceName = ConfigurationManager.AppSettings["DatatecServiceName"];
            

                ServiceController[] serviceControllers = ServiceController.GetServices(_datatecServerName);
               _datatecServiceController = serviceControllers.Where(x => x.ServiceName == _datatecServiceName).Single();

                return true;
            }
            catch (System.ComponentModel.Win32Exception ex)
            {

                notificationService.SendNotification(ex.ToString());
                logService.Log(LogLevel.Error, ex.ToString());
            }
           catch (ConfigurationErrorsException ex)
            {

                notificationService.SendNotification(ex.ToString());
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (ArgumentNullException ex)
            {

                logService.Log(LogLevel.Info, String.Format("Service {0} is not installed in {1}.", _datatecServiceName, _datatecServerName));
                notificationService.SendNotification(String.Format("Service {0} is not installed in {1}.", _datatecServiceName, _datatecServerName));

                notificationService.SendNotification(ex.ToString());
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (ArgumentException ex)
            {

                notificationService.SendNotification(ex.ToString());
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (InvalidOperationException ex)
            {

                notificationService.SendNotification(ex.ToString());
                logService.Log(LogLevel.Error, ex.ToString());
            }
            return false;

        }

        public DatatecMonitor(INotificationService notificationService,ILogService logService,ITimeService timeService)
        {
             _status = Status.Stopped;
            this.notificationService = notificationService;
            this.logService = logService;
            this.timeService = timeService;
        }
                                  
        private bool isDatatecDown()
        {
            return !_datatecServiceController.Status.Equals(ServiceControllerStatus.Running);
        }

        private void RestartWindowsService()
        {
            try
            {
                if ((_datatecServiceController.Status.Equals(ServiceControllerStatus.Running)) || (_datatecServiceController.Status.Equals(ServiceControllerStatus.StartPending)))
                {
                    _datatecServiceController.Stop();
                    logService.Log(LogLevel.Info, String.Format("Stopping service {0}", _datatecServiceName));
                    _datatecServiceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.Parse("00:02:00"));


                    _datatecServiceController.Start();
                    logService.Log(LogLevel.Info, String.Format("Starting service {0}", _datatecServiceName));
                    _datatecServiceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.Parse("00:04:00"));
                    if (_datatecServiceController.Status.Equals(ServiceControllerStatus.Running))
                    {
                        logService.Log(LogLevel.Info, String.Format("Service {0} started", _datatecServiceName));
                        notificationService.SendNotification(String.Format("Service {0} started", _datatecServiceName));
                    }
                    else
                    {
                        logService.Log(LogLevel.Info, String.Format("Service {0} not started", _datatecServiceName));
                        notificationService.SendNotification(String.Format("Service {0} not started", _datatecServiceName));

                    }
                }

                if (_datatecServiceController.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    _datatecServiceController.Start();
                    logService.Log(LogLevel.Info, String.Format("Starting service {0}", _datatecServiceName));
                    _datatecServiceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.Parse("00:04:00"));

                    if (_datatecServiceController.Status.Equals(ServiceControllerStatus.Running))
                    {
                        logService.Log(LogLevel.Info, String.Format("Service {0} started", _datatecServiceName));
                        notificationService.SendNotification(String.Format("Service {0} started", _datatecServiceName));
                    }
                    else
                    {
                        logService.Log(LogLevel.Info, String.Format("Service {0} not started", _datatecServiceName));
                        notificationService.SendNotification(String.Format("Service {0} not started", _datatecServiceName));

                    }
                }

            }
            catch (System.ServiceProcess.TimeoutException toe)
            {
                notificationService.SendNotification(toe.ToString());
                logService.Log(LogLevel.Error, toe.ToString());

            }
            catch (System.ComponentModel.Win32Exception ew)
            {
                notificationService.SendNotification(ew.ToString());
                logService.Log(LogLevel.Error, ew.ToString());
            }
            catch (InvalidOperationException ioe)
            {
                notificationService.SendNotification(ioe.ToString());
                logService.Log(LogLevel.Error, ioe.ToString());
            }
            catch (Exception ex)
            {

                notificationService.SendNotification(ex.ToString());
                logService.Log(LogLevel.Error, ex.ToString());
            }
        }

        
        private void CheckServiceStatus()
        {
            if (_status == Status.Started)
            {
                logService.Log(LogLevel.Debug, "  ");
                logService.Log(LogLevel.Debug,"Checking service status");

                if (isDatatecDown())
                {
                    RestartWindowsService();
                }
                else
                if (timeService.IsActiveHours() && timeService.EncuentraSilencioAnormal())
                {
                    TimeSpan time = timeService.TimeSpanSinceLastEvent();
                    logService.Log(LogLevel.Warn, String.Format("Tiempo desde el ultimo evento: {0}", time.ToString()));
                    notificationService.SendNotification(String.Format("Tiempo desde el ultimo evento: {0}", time.ToString()));
                }
         
            }
            else
            {
                logService.Log(LogLevel.Info, String.Format("Service {0} Down.", _datatecServiceName));
                notificationService.SendNotification(String.Format("Service {0} Down.", _datatecServiceName));
            }
        }


        public void IniciarTareasMonitoras()
        {
            try
            {
                
                _tareas = new List<Tarea>();
                foreach (var periodo in getPeriodos())
                {
                    Tarea t = new Tarea(CheckServiceStatus, periodo);
                    t.Iniciar();
                   _tareas.Add(t);
                }

            }
            catch (ArgumentNullException ane)
            {
                notificationService.SendNotification(ane.ToString());
                logService.Log(LogLevel.Error, ane.ToString());
            }
            catch (ArgumentOutOfRangeException aoe)
            {
                notificationService.SendNotification(aoe.ToString());
                logService.Log(LogLevel.Error, aoe.ToString());

            }
        }

        public void DetenerTareasMonitoras()
        {
           foreach (Tarea tarea in _tareas)
             tarea.Detener();
        }
        private void CrearTareaInicializadora()
        {
            string FechaStr = String.Format("{0} {1}", DateTime.Now.AddDays(1).ToString("dd/MM/yyyy"), "01:00");
            DateTime hi = DateTime.Parse(FechaStr);
            DateTime hf = hi.AddYears(10);

            Periodo p = new Periodo()
            {
                HoraInicio = hi,
                HoraFin = hf,
                IntervaloRevision = TimeSpan.Parse("24:00:00"),
                Nombre = "TareaInicializadora",
            };
            tareaInicializadora = new Tarea(IniciarTareasMonitoras, p);
            tareaInicializadora.Iniciar();
        }
       
        public void Start()
        {
           
            if (InitializeService())
            {

                _status = Status.Started;
                logService.Log(LogLevel.Info, "Datatec Monitor Started");
                IniciarTareasMonitoras();
                CrearTareaInicializadora();
                
            }
        
        }

        public void Stop()
        {
            if (_status == Status.Started)
            {
                DetenerTareasMonitoras();
                tareaInicializadora.Detener();
                logService.Log(LogLevel.Info, "Datatec Monitor Stoped");
            }
            else
            {
                logService.Log(LogLevel.Info, "Datatec Monitor already stopped");
            }

        }
  
    }
}
