using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Datatec.Infrastructure;
using Datatec.Services;

namespace Datatec.Monitor
{
    public partial class Service1 : ServiceBase
    {
        private readonly IDatatecMonitor serviceMonitor;

        public Service1(IDatatecMonitor serviceMonitor)
        {
            InitializeComponent();
            this.serviceMonitor = serviceMonitor;
        }

        protected override void OnStart(string[] args)
        {
            serviceMonitor.Start();
        }

        protected override void OnStop()
        {
            serviceMonitor.Stop();

        }
    }
}
