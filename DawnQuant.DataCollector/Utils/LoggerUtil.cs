using Autofac;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Utils
{
    public class LoggerUtil<T> : ILogger<T>
    {
        ILogger<T> _logger;
        public LoggerUtil()
        {
            _logger= IOCUtil.Container.Resolve<ILoggerFactory>().CreateLogger<T>();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
