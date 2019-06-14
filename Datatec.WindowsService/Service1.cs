
using Datatec.Persistence;
using Datatec.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Datatec.WindowsService
{
    public partial class Service1 : ServiceBase
    {
        private readonly ILogService logService;
        private readonly IDatabaseService dbService;
        private readonly IDatatecService service;

        public Service1(ILogService logService, IDatabaseService dbService, IDatatecService service)
        {
            this.logService = logService;
            this.dbService = dbService;
            this.service = service;
            InitializeComponent();
       
        }

        protected override void OnStart(string[] args)
        {
            service.Start();
        }

        protected override void OnStop()
        {
            service.Stop();
        }
    }
}
