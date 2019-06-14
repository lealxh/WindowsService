using Datatec.Implementation;
using Datatec.Persistence;
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
            IDatatecService datatecService = new DatatecService(logService,dbService);
            
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1(logService,dbService,datatecService)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
