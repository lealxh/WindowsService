using Datatec.DTO;
using Datatec.Infrastructure;
using Datatec.Services;
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
        public readonly TimeSpan _maxTimeSpan;
        private readonly ILogService _logService;
        private readonly IDatabaseService _dbService;
        private readonly ITimeService timeService;
        private readonly INotificationService notificationService;
        private FileSystemWatcher _watcher;
        private readonly string _pathToWatch;
        private readonly string _nameofFile;

        private Timer _tareaWatcher;


        public TimeSpan TimeSinceLastEvent
        {   get
            {
                TimeSpan t = (timeService.GetCurrentTime() - _lastEventTime);
                return t;
            }
        }

        private DateTime _lastEventTime;
        private Status _status;


        public DatatecService(ILogService logService, IDatabaseService dbService,ITimeService timeService,INotificationService notificationService)
        {
            try
            {


                this._logService = logService;
                this._dbService = dbService;
                this.timeService = timeService;
                this.notificationService = notificationService;
                _maxTimeSpan = TimeSpan.Parse(ConfigurationManager.AppSettings["MaxTimeSpan"]);
                _pathToWatch = ConfigurationManager.AppSettings["PathToWatch"];
                _nameofFile = ConfigurationManager.AppSettings["FileName"];
                _lastEventTime = DateTime.Now;
            }
            catch (ConfigurationErrorsException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
                notificationService.SendNotification(ex.ToString());
            }
        }


        private void OnChanged(object source, FileSystemEventArgs e)
        {
            _lastEventTime = DateTime.Now;
            string lastLine=ReadInput();
            if(lastLine!=null)
            WriteOutPut(lastLine);

        }

        private void OnError(object source, ErrorEventArgs e)
        {
            _logService.Log(LogLevel.Error, "Error lanzado por el watcher");
            _logService.Log(LogLevel.Error, e.GetException().ToString());
            notificationService.SendNotification("Error detectado por el watcher: "+e.GetException().Message);

            CreateWatcher();
        }

        void WatcherMonitorCallback()
        {
            if (_status == Status.Started)
            {
                if (timeService.IsWorkingHours() && TimeSinceLastEvent > _maxTimeSpan)
                {
                    _logService.Log(LogLevel.Warn, String.Format("Tiempo desde el ultimo evento: {0}", TimeSinceLastEvent.ToString(@"hh\:mm\:ss")));
                    notificationService.SendNotification(String.Format("Tiempo desde el ultimo evento: {0}", TimeSinceLastEvent.ToString(@"hh\:mm\:ss")));

                    if (!Directory.Exists(_pathToWatch))
                    {
                        _logService.Log(LogLevel.Warn, String.Format("Directorio {0} no disponible", _pathToWatch));
                        notificationService.SendNotification(String.Format("Directorio {0} no disponible", _pathToWatch));
                    }
                }
            }
            else
            {
                DisposeWatcherMonitor();
            }
        }

        private void CreateWatcherMonitor()
        {
            try
            {

                var startTimeSpan = TimeSpan.Zero;
                var periodTimeSpan = _maxTimeSpan;
                 _tareaWatcher = new System.Threading.Timer( (e) => { WatcherMonitorCallback();}, 
                 null, 
                 startTimeSpan, 
                 periodTimeSpan);


            }
            catch (ArgumentNullException ane)
            {
                notificationService.SendNotification(ane.ToString());
                _logService.Log(LogLevel.Error, ane.ToString());
            }
            catch (ArgumentOutOfRangeException aoe)
            {
                notificationService.SendNotification(aoe.ToString());
                _logService.Log(LogLevel.Error, aoe.ToString());

            }

        }

        private void DisposeWatcherMonitor()
        {
            _tareaWatcher.Change(Timeout.Infinite, Timeout.Infinite);
            _tareaWatcher.Dispose();
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
        private void CreateWatcher()
        {
            try
            {
                _watcher = new FileSystemWatcher(_pathToWatch, _nameofFile);
                _watcher.NotifyFilter = NotifyFilters.LastWrite;
                _watcher.Changed += OnChanged;
                _watcher.Error += new ErrorEventHandler(OnError);
                _watcher.EnableRaisingEvents = true;
                _logService.Log(LogLevel.Info, "FileSystemWatcher inicializado en " + _pathToWatch + "\\" + _nameofFile);
            }
          

            catch (ArgumentNullException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (ArgumentException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (PathTooLongException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (ObjectDisposedException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (PlatformNotSupportedException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (FileNotFoundException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }


        }
        private void DisposeWatcher()
        {
            _logService.Log(LogLevel.Info, "FileSystemWatcher deshabilitado");
            try
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();

            }
            catch (ArgumentException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (ObjectDisposedException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (PlatformNotSupportedException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }
            catch (FileNotFoundException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }
          

        }

        public void Start()
        {
            _status = Status.Started;
            _logService.Log(LogLevel.Info, "Servicio Iniciado");
            CreateWatcher();
            CreateWatcherMonitor();
        }

        public void Stop()
        {
            this._status = Status.Stopped;
            
            DisposeWatcherMonitor();
            DisposeWatcher();
            _logService.Log(LogLevel.Info, "Servicio Detenido");
        }
        public void WriteOutPut(String lastLine)
        {
            try
            {

            
                string spName= ConfigurationManager.AppSettings["StoredProcedureName"];

                string FechaStr = lastLine.Substring(0, 10).Trim();
                FechaStr = FechaStr.Substring(FechaStr.Length - 6, 6);
                FechaStr = FechaStr.Substring(0, 2) + ":" + FechaStr.Substring(2, 2) + ":" + FechaStr.Substring(4, 2);
                FechaStr = DateTime.Now.ToString("dd/MM/yyyy") +" " +FechaStr;

                DateTime FechaDt = DateTime.Now;

                DateTime.TryParseExact(FechaStr, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture,DateTimeStyles.None, out FechaDt);

           
                string valorStr = lastLine.Substring(10, 9).Trim().Replace('.', ',');
                decimal valorDec = 0;
                decimal.TryParse(valorStr, out valorDec);

                string factorStr = ConfigurationManager.AppSettings["Factor"];
                decimal factorDec = 0;
                decimal.TryParse(factorStr, out factorDec);

                var data = new PuntaDolarDTO()
                {
                    Fecha= FechaDt,
                    Precio=valorDec,
                    Factor=factorDec,
                    Moneda="USD"
                };
        
                _dbService.ExecuteQuery(spName, _dbService.CreateParameters(data));
                _logService.Log(LogLevel.Info, "Informacion enviada a Base de datos");
            }
            catch (ConfigurationErrorsException ex)
            {
                _logService.Log(LogLevel.Error, ex.ToString());
            }

        }
    }
}

