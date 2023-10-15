using Microsoft.Extensions.Logging;

namespace TeamGPT.Utilities
{
    public class Logger
    {
        private readonly LoggingConfiguration _loggingConfig;
        private readonly ILogger _logger;
        public enum CustomLogLevel
        {
            Trace,
            Debug,
            Information,
            Warning,
            Error,
            Critical
        }

        public class LoggingConfiguration
        {
            public string Default { get; set; }
            public string ErrorHandler { get; set; }
        }

        public Logger(LoggingConfiguration loggingConfig, ILogger<Logger> logger)
        {
            this._loggingConfig = loggingConfig;
            this._logger = logger;
        }

        public void Log(CustomLogLevel logLevel, string name, string message)
        {
            var formattedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{name}] -- {message}";

            switch (logLevel)
            {
                case CustomLogLevel.Trace:
                    _logger.LogTrace(formattedMessage);
                    break;
                case CustomLogLevel.Debug:
                    _logger.LogDebug(formattedMessage);
                    break;
                case CustomLogLevel.Information:
                    _logger.LogInformation(formattedMessage);
                    break;
                case CustomLogLevel.Warning:
                    _logger.LogWarning(formattedMessage);
                    break;
                case CustomLogLevel.Error:
                    _logger.LogError(formattedMessage);
                    break;
                case CustomLogLevel.Critical:
                    _logger.LogCritical(formattedMessage);
                    break;
                default:
                    _logger.LogInformation(formattedMessage);
                    break;
            }
        }

        public void LogDebug(string Name, string message)
        {
            if (_loggingConfig.Default.ToLower() == "debug")
                _logger.LogDebug(message);
        }

        public void LogInformation(string name, string message)
        {
            if (_loggingConfig.Default.ToLower() == "information")
                _logger.LogInformation(message);
        }

        public void LogWarning(Exception ex)
        {
            if (_loggingConfig.ErrorHandler.ToLower() == "warning")
                _logger.LogWarning(ex, ex.Message);
        }

        public void LogError(Exception ex)
        {
            if (_loggingConfig.ErrorHandler.ToLower() == "error")
                _logger.LogError(ex, ex.Message);
        }

        public void LogCritical(Exception ex)
        {
            if (_loggingConfig.ErrorHandler.ToLower() == "critical")
                _logger.LogCritical(ex, ex.Message);
        }
    }
}
