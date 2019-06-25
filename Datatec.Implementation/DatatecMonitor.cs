using Datatec.Infrastructure;
using Datatec.Services;
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
        
        public readonly TimeSpan _timeSpanToCheck;
        private readonly INotificationService notificationService;
        private readonly ILogService logService;
        private Status _status;
        private readonly string _datatecServerName;
        private readonly string _datatecServiceName;
        private readonly ServiceController _datatecServiceController;
        private Timer _tareaMonitor;

        public DatatecMonitor(INotificationService notificationService,ILogService logService)
        {
            try
            {
                    _timeSpanToCheck = TimeSpan.Parse(ConfigurationManager.AppSettings["TimesPanToCheck"]);
                    _datatecServerName = ConfigurationManager.AppSettings["DatatecServerName"];
                    _datatecServiceName = ConfigurationManager.AppSettings["DatatecServiceName"];

                    ServiceController[] serviceControllers = ServiceController.GetServices(_datatecServerName);

                    _datatecServiceController  = serviceControllers.Where(x => x.ServiceName == _datatecServiceName).SingleOrDefault();
            
            
                    this.notificationService = notificationService;
                    this.logService = logService;
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
        }


        public bool isDatatecDown()
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
               if(_status == Status.Started)
                if (isDatatecDown())
                {
                    RestartWindowsService();
                   
                }
                else
                {
                    logService.Log(LogLevel.Info, String.Format("Service {0} Ok.", _datatecServiceName));
                    notificationService.SendNotification(String.Format("Service {0} Ok.", _datatecServiceName));
                }

        }

        
        public void CreateTareaMonitor()
        {
            try
            {
                var startTimeSpan = TimeSpan.Zero;
                var periodTimeSpan = _timeSpanToCheck;
                _tareaMonitor = new Timer((e) =>
                {
                    CheckServiceStatus();
                }, 
                null, 
                startTimeSpan, 
                periodTimeSpan);

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
        void DisposeTareaMonitor()
        {
            _tareaMonitor.Change(Timeout.Infinite, Timeout.Infinite);
            _tareaMonitor.Dispose();
        }
        public void Start()
        {
           
            if (_datatecServiceController != null)
            {
                _status = Status.Started;
                logService.Log(LogLevel.Info, "Datatec Monitor Started");

                CreateTareaMonitor();
            }
            else
            {
                logService.Log(LogLevel.Info,String.Format("Service {0} is not installed in {1}.", _datatecServiceName, _datatecServerName));
                notificationService.SendNotification(String.Format("Service {0} is not installed in {1}.", _datatecServiceName,_datatecServerName));
            }
        
        }

        public void Stop()
        {
            _status = Status.Started;
            DisposeTareaMonitor();
            logService.Log(LogLevel.Info, "Datatec Monitor Stoped");
         
        }
    }
}
