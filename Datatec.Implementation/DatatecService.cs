using Datatec.DTO;
using Datatec.Infrastructure;
using Datatec.Interfaces;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Datatec.Implementation
{
    public class DatatecService : IDatatecService
    {
        
        private readonly ILogService logService;
        private readonly IDatabaseService dbService;
        private readonly INotificationService notificationService;
        private readonly ITimeService timeService;
        private FileSystemWatcher _watcher;
        private string _pathToWatch;
        private string _nameofFile;
        private string _moneda;
        private string _spName;
        private string _factor;
 
        private Status _status;

        private bool InitializeService()
        {
            try
            {
                _pathToWatch = ConfigurationManager.AppSettings["PathToWatch"];
                _nameofFile = ConfigurationManager.AppSettings["FileName"];
                _moneda = ConfigurationManager.AppSettings["Moneda"];
                _spName = ConfigurationManager.AppSettings["StoredProcedureName"];
                _factor = ConfigurationManager.AppSettings["Factor"];
                return true;
            }
            catch (ConfigurationErrorsException ex)
            {
                logService.Log(LogLevel.Error, "Error en la configuracion del servicio ");
                logService.Log(LogLevel.Error, ex.ToString());
                notificationService.SendNotification(ex.ToString());
            }
            return false;

        }
        private bool CreateWatcher()
        {
            
            try
            {
                if (Directory.Exists(_pathToWatch))
                {
                    _watcher = new FileSystemWatcher(_pathToWatch, _nameofFile);
                    _watcher.NotifyFilter = NotifyFilters.LastWrite;
                    _watcher.Changed += OnChanged;
                    _watcher.Error += new ErrorEventHandler(OnError);
                    _watcher.EnableRaisingEvents = true;
                    logService.Log(LogLevel.Debug, "FileSystemWatcher inicializado en " + _pathToWatch + "\\" + _nameofFile);
                    return true;

                }
                else
                {
                    logService.Log(LogLevel.Warn, "El directorio " + _pathToWatch + "\\" + _nameofFile+" no esta disponible");
                }

        
            }


            catch (ArgumentNullException ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (ArgumentException ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (PathTooLongException ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (ObjectDisposedException ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (PlatformNotSupportedException ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (FileNotFoundException ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (Exception ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }

            return false;

        }
        private void DisposeWatcher()
        {
            logService.Log(LogLevel.Debug, "FileSystemWatcher deshabilitado");
            try
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();

            }
            catch (ArgumentException ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (ObjectDisposedException ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (PlatformNotSupportedException ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (FileNotFoundException ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }


        }

        public DatatecService(ILogService logService, IDatabaseService dbService,INotificationService notificationService,ITimeService timeService)
        {
            this.logService = logService;
            this.dbService = dbService;
            this.notificationService = notificationService;
            this.timeService = timeService;
            _status = Status.Stopped;

        }

        public string ReadInput()
        {
            try
            {
                String path = _pathToWatch + @"\" + _nameofFile;
                string lastline = File.ReadLines(path).Last();
                logService.Log(LogLevel.Debug, lastline);
                return lastline;
            }
            catch (System.IO.IOException ioe)
            {
                logService.Log(LogLevel.Error, ioe.ToString());
            }
            catch (Exception ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
            return null;
        }
        public void WriteOutPut(String lastLine)
        {
            try
            {
                string FechaStr = lastLine.Substring(0, 10).Trim();
                FechaStr = FechaStr.Substring(FechaStr.Length - 6, 6);
                FechaStr = FechaStr.Substring(0, 2) + ":" + FechaStr.Substring(2, 2) + ":" + FechaStr.Substring(4, 2);
                FechaStr = DateTime.Now.ToString("dd/MM/yyyy") + " " + FechaStr;
                logService.Log(LogLevel.Debug,"Fecha String: "+FechaStr);

                DateTime FechaDt = DateTime.Now;
                DateTime.TryParse(FechaStr,out FechaDt);

                logService.Log(LogLevel.Debug, "Fecha DateTime: " + FechaDt.ToString());

                string valorStr = lastLine.Substring(10, 9).Trim().Replace('.', ',');
                decimal valorDec = 0;
                decimal.TryParse(valorStr, out valorDec);

            
                decimal factorDec = 0;
                decimal.TryParse(_factor, out factorDec);

                var data = new PuntaDolarDTO()
                {
                    Fecha = FechaDt,
                    Precio = valorDec,
                    Factor = factorDec,
                    Moneda = _moneda
                };
                
                dbService.ExecuteQuery(_spName, dbService.CreateParameters(data));
                logService.Log(LogLevel.Debug, "Informacion enviada a Base de datos");
            }
            catch (Exception ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
          

        }

        public void Start()
        {
            if(InitializeService())
            {
                _status = Status.Started;
                timeService.WriteLastEventTime();
                logService.Log(LogLevel.Info, "Servicio Iniciado");
                CreateWatcher();
          
            }

        }
       public void Stop()
        {
            try
            {
                this._status = Status.Stopped;
                DisposeWatcher();
                logService.Log(LogLevel.Info, "Servicio Detenido");
            }
            catch (Exception ex)
            {
                logService.Log(LogLevel.Error, ex.ToString());
            }
           
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            timeService.WriteLastEventTime();
            string lastLine = ReadInput();
            if (lastLine != null)
                WriteOutPut(lastLine);

        }
        private Timer _tareaMonitor;
        void DisposeReinitTask()
        {
            _tareaMonitor.Change(Timeout.Infinite, Timeout.Infinite);
            _tareaMonitor.Dispose();
        }

        private void CreateReinitTask()
        {
            try
            {
                var startTimeSpan = TimeSpan.FromMinutes(1);
                var periodTimeSpan = TimeSpan.Parse("00:01:00");
                _tareaMonitor = new Timer((e) =>
                {
                    if (Directory.Exists(_pathToWatch))
                    {
                        if(CreateWatcher())
                            DisposeReinitTask();
                    }
                    else
                    {
                        logService.Log(LogLevel.Debug, "El directorio " + _pathToWatch + "\\" + _nameofFile + " no esta disponible");
                    }

                },
                null,
                startTimeSpan,
                periodTimeSpan);

            }
            catch (ArgumentNullException ane)
            {
                notificationService.SendNotification(ane.ToString());
                logService.Log(LogLevel.Error, ane.ToString());
            }
            catch (ArgumentOutOfRangeException aoe)
            {
                notificationService.SendNotification(aoe.ToString());
                logService.Log(LogLevel.Error, aoe.ToString());

            }

        }

        private void OnError(object source, ErrorEventArgs e)
        {
            logService.Log(LogLevel.Error, "Error detectado por el watcher: "+e.GetException().Message);
            CreateReinitTask();
        }



    }
}

