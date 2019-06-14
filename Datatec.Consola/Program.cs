using Datatec.Implementation;
using Datatec.Persistence;
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
            DatatecService service = new DatatecService(logService, dbService);
            service.Start();
      
        }
        static void Main(string[] args)
        {
            Program p = new Program();
            Console.ReadLine();

            
            
        }
    }
}
