using Datatec.Implementation;
using Datatec.DTO;
using Datatec.Infrastructure;
using Datatec.Interfaces;
using System;
using Autofac;
using System.Configuration;
using System.Collections.Generic;

namespace Datatec.Consola
{
    class Program
    {
        private static IContainer Container { get; set; }

        public static List<Periodo> getPeriodos()
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
                var service = scope.Resolve<IDatatecMonitor>();
                service.Start();
                Console.ReadLine();
                service.Stop();

            }

            //var periodos=getPeriodos();

            //Tarea T1 = new Tarea(CallBack1, periodos[0]);
            //T1.Iniciar();

            //Tarea T2 = new Tarea(CallBack2, periodos[1]);
            //T2.Iniciar();

            //Tarea T3 = new Tarea(CallBack3, periodos[2]);
            //T3.Iniciar();


            //Console.ReadLine();
        }
        public static void CallBack1()
        {
            Console.WriteLine("Tarea 1");
        }

        public static void CallBack2()
        {
            Console.WriteLine("Tarea 2");
        }

        public static void CallBack3()
        {
            Console.WriteLine("Tarea 3");
        }



    }
}
