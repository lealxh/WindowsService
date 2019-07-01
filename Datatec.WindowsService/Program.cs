using Datatec.Implementation;
using Datatec.Infrastructure;
using Datatec.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Datatec.WindowsService
{
    static class Program
    {
        private static IContainer Container { get; set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            var builder = new ContainerBuilder();
            builder.RegisterType<LogService>().As<ILogService>();
            builder.RegisterType<DatabaseService>().As<IDatabaseService>();
            builder.RegisterType<SlackClient>().As<ISlackClient>();
            builder.RegisterType<FileService>().As<IFileService>();
            builder.RegisterType<TimeService>().As<ITimeService>();
            builder.RegisterType<NotificationService>().As<INotificationService>();
            builder.RegisterType<DatatecService>().As<IDatatecService>();
            Container = builder.Build();

            using (var scope = Container.BeginLifetimeScope())
            {
                var logService = scope.Resolve<ILogService>();
                var service = scope.Resolve<IDatatecService>();

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new Service1(logService,service)
                };
                ServiceBase.Run(ServicesToRun);


            }



            
        }
    }
}
