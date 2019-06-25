﻿using Datatec.Implementation;
using Datatec.Infrastructure;
using Datatec.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ILogService logService = new LogService();
            IDatabaseService dbService = new DatabaseService(logService);
            ITimeService timeService = new TimeService();
            ISlackClient slackClient = new SlackClient(logService);
            INotificationService notificationService = new NotificationService(slackClient);
            DatatecService service = new DatatecService(logService, dbService, timeService, notificationService);

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1(logService,service)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
