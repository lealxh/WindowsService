using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Datatec.Infrastructure;
using log4net;
using log4net.Config;

namespace Datatec.Infrastructure
{
    public class LogService: ILogService
    {
        private readonly log4net.ILog _logger;
        public LogService()
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()));
            this._logger = log4net.LogManager.GetLogger(Assembly.GetEntryAssembly(), "Datatec");
        }

        public void Log(LogLevel level, object message)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    if(_logger.IsDebugEnabled)
                    _logger.Debug(message);
                    break;
                case LogLevel.Info:
                    if (_logger.IsInfoEnabled)
                        _logger.Info(message);
                    break;
                case LogLevel.Warn:
                    if (_logger.IsWarnEnabled)
                        _logger.Warn(message);
                    break;
                case LogLevel.Error:
                    if (_logger.IsErrorEnabled)
                        _logger.Error(message);
                    break;
                case LogLevel.Fatal:
                    if (_logger.IsFatalEnabled)
                        _logger.Fatal(message);
                    break;
                default:
                    break;
            }
        }
    }
}
