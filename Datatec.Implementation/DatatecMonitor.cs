using Datatec.Infrastructure;
using Datatec.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
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
        public DatatecMonitor(INotificationService notificationService,ILogService logService)
        {
            
            _timeSpanToCheck = TimeSpan.Parse(ConfigurationManager.AppSettings["TimesPanToCheck"]);
            _datatecServerName = ConfigurationManager.AppSettings["DatatecServerName"];
            _datatecServiceName = ConfigurationManager.AppSettings["DatatecServiceName"];

            ServiceController[] serviceControllers = ServiceController.GetServices(_datatecServerName);

            _datatecServiceController  = serviceControllers.Where(x => x.ServiceName == _datatecServiceName).SingleOrDefault();
            
            
            this.notificationService = notificationService;
            this.logService = logService;
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
                    _datatecServiceController.WaitForStatus(ServiceControllerStatus.Stopped,TimeSpan.Parse("00:02:00"));
                    

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
                
                if(_datatecServiceController.Status.Equals(ServiceControllerStatus.Stopped))
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
            catch(Exception ex)
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

        public void Start()
        {
            _status = Status.Started;
            logService.Log(LogLevel.Info, "Datatec Monitor Started");
            

            if (_datatecServiceController != null)
            {
                var startTimeSpan = TimeSpan.Zero;
                var periodTimeSpan = _timeSpanToCheck;
                var timer = new System.Threading.Timer((e) =>
                {
                    CheckServiceStatus();
                }, null, startTimeSpan, periodTimeSpan);
            }
            else
            {
                logService.Log(LogLevel.Info,String.Format("Service {0} is not installed in {1}.", _datatecServiceName, _datatecServerName));
                notificationService.SendNotification(String.Format("Service {0} is not installed in {1}.", _datatecServiceName,_datatecServerName));
            }
        
        }

        public void Stop()
        {
            logService.Log(LogLevel.Info, "Datatec Monitor Stoped");
            _status = Status.Started;
        }
    }
}
