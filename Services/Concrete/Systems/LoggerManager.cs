using Microsoft.Extensions.Logging;
using NLog;
using Services.Abstract.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILogger = NLog.ILogger;

namespace Services.Concrete.Systems
{
    public class LoggerManager : ILoggerService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        public void LogDebug(string message) => logger.Debug(message);

        public void LogError(string message) => logger.Error(message);

        public void LogInfo(string message) => logger.Info(message);

        public void LogWarning(string message) => logger.Warn(message);
        public void LogInformation(string message) => logger.Warn(message);

    }
}
