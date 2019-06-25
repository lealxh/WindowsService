using Datatec.Implementation;
using Datatec.Infrastructure;
using Datatec.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Consola
{
    class Program
    {
        public Program()
        {
            ILogService logService = new LogService();
            IDatabaseService dbService = new DatabaseService(logService);
            ITimeService timeService = new TimeService();
            ISlackClient slackClient = new SlackClient(logService);
            INotificationService notificationService = new NotificationService(slackClient);
           // DatatecService service = new DatatecService(logService, dbService,timeService,notificationService);
            DatatecMonitor service = new DatatecMonitor(notificationService,logService);
            service.Start();
      
        }
        static void Main(string[] args)
        {
            Program p = new Program();
            Console.ReadLine();

            
            
        }
    }
}
