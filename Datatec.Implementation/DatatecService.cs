using Datatec.DTO;
using Datatec.Persistence;
using Datatec.Services;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace Datatec.Implementation
{
    public class DatatecService : IDatatecService
    {
        private readonly ILogService _logService;
        private readonly IDatabaseService _dbService;
        private FileSystemWatcher _watcher;
        private readonly string _pathToWatch;
        private readonly string _nameofFile;


        public DatatecService(ILogService logService, IDatabaseService dbService)
        {
            this._logService = logService;
            this._dbService = dbService;
            this._pathToWatch= ConfigurationManager.AppSettings["PathToWatch"];
            this._nameofFile = ConfigurationManager.AppSettings["FileName"];
            _watcher = new FileSystemWatcher(_pathToWatch, _nameofFile);

        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            string lastLine=ReadInput();
            if(lastLine!=null)
            WriteOutPut(lastLine);

        }

        public string ReadInput()
        {
            try
            {
                String path = _pathToWatch + @"\" + _nameofFile;
                string lastline = File.ReadLines(path).Last();
                _logService.Log(LogLevel.Info, lastline);
                return lastline;
            }
            catch (System.IO.IOException ioe)
            {
                _logService.Log(LogLevel.Error, ioe.ToString());
            }
            catch (Exception ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }
            return null;
      }

        public void Start()
        {
           _watcher.NotifyFilter = NotifyFilters.LastWrite;
           _watcher.Changed += OnChanged;
           _watcher.EnableRaisingEvents = true;
           _logService.Log(LogLevel.Info, "Servicio Iniciado");
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _logService.Log(LogLevel.Info, "Servicio Detenido");
        }
        public void WriteOutPut(String lastLine)
        {
             string spName= ConfigurationManager.AppSettings["StoredProcedureName"];

            string valorStr = lastLine.Substring(10, 9).Trim().Replace('.', ',');
            decimal valorDec = 0;
            decimal.TryParse(valorStr, out valorDec);

            string factorStr = ConfigurationManager.AppSettings["Factor"];
            decimal factorDec = 0;
            decimal.TryParse(factorStr, out factorDec);


            var data = new PuntaDolarDTO()
            {
                Fecha=DateTime.Now,
                Precio=valorDec,
                Factor=factorDec,
                Moneda="USD"
            };
        
            _dbService.ExecuteQuery(spName, _dbService.CreateParameters(data));
            _logService.Log(LogLevel.Info, "Informacion enviada a Base de datos");

        }
    }
}
