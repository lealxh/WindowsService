using Datatec.Implementation;
using Datatec.Infrastructure;
using Datatec.Services;
using System;
using Autofac;

namespace Datatec.Consola
{
    class Program
    {
        private static IContainer Container { get; set; }

        static void Main(string[] args)
        {

            var builder = new ContainerBuilder();
            builder.RegisterType<LogService>().As<ILogService>();
            builder.RegisterType<FileService>().As<IFileService>();
            builder.RegisterType<DatabaseService>().As<IDatabaseService>();
            builder.RegisterType<SlackClient>().As<ISlackClient>();
            builder.RegisterType<TimeService>().As<ITimeService>();
            builder.RegisterType<NotificationService>().As<INotificationService>();
            builder.RegisterType<DatatecService>().As<IDatatecService>();
            builder.RegisterType<DatatecMonitor>().As<IDatatecMonitor>();
            Container = builder.Build();

            using (var scope = Container.BeginLifetimeScope())
            {
                var service = scope.Resolve<IDatatecService>();
                service.Start();
                Console.ReadLine();
                service.Stop();

            }

     
        }

            

          
    }
}
