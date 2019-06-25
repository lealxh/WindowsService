using Datatec.Implementation;
using Datatec.Infrastructure;
using Datatec.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Monitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ILogService logService = new LogService();
            ISlackClient slackClient = new SlackClient(logService);
            

            INotificationService notificationService = new NotificationService(slackClient);
            IDatatecMonitor service = new DatatecMonitor(notificationService,logService);
         
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1(service)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
