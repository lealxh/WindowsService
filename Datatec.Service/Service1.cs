
using Datatec.Infrastructure;
using Datatec.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.Service
{
    public partial class Service1 : ServiceBase
    {
        private readonly ILogService logService;
        private readonly IDatatecService service;

        public Service1(ILogService logService,  IDatatecService service)
        {
            this.AutoLog = true;
            this.logService = logService;
            this.service = service;
            InitializeComponent();
       
        }

        protected override void OnStart(string[] args)
        {
            service.Start();
            
        }

        protected override void OnPause()
        {
            logService.Log(LogLevel.Info, "Evento Pause");
            service.Stop();

        }

        protected override void OnContinue()
        {
            logService.Log(LogLevel.Info, "Evento Continue");
            service.Start();
           
        }

        protected override void OnShutdown()
        {
            logService.Log(LogLevel.Info, "Evento Shutdown");
            service.Stop();
         
        }

        protected override void OnStop()
        {
           service.Stop();
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            logService.Log(LogLevel.Info, "Cambio de sesion: " + changeDescription.Reason);
        
        }



    }
}
