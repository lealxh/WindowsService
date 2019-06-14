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
            WriteOutPut(lastLine);

        }

        public string ReadInput()
        {
                try
                {
                    String path = _pathToWatch + @"\" + _nameofFile;
                    string lastline = File.ReadLines(path).Last();
                    _logService.Log(LogLevel.Info,lastline);
                    return lastline;
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
            _dbService.ExecuteQuery(spName, _dbService.CreateParameters(lastLine));
            _logService.Log(LogLevel.Info, "Informacion enviada a Base de datos");

        }
    }
}
