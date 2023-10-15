using Microsoft.Extensions.Logging;
using Serilog;

namespace TeamGPT.Utilities
{
    public class Logger
    {
        private readonly LoggingConfiguration _loggingConfig;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public enum CustomLogLevel
        {
            Trace,
            Debug,
            Information,
            Warning,
            Error,
            Critical
        }

        private Serilog.ILogger _serilogLogger; // Remove readonly

        public class LoggingConfiguration
        {
            public string Default { get; set; }
            public string ErrorHandler { get; set; }
        }

        public Logger(Microsoft.Extensions.Logging.ILogger<Logger> logger)
        {
            this._logger = logger;
            InitializeLogger();
        }

        private void InitializeLogger()
        {
            _serilogLogger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        public void ConfigureLogFile(string logFilePath)
        {
            // Configure the log file dynamically based on the provided log file path
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day);

            // Update the Serilog logger without disposing the previous one
            _serilogLogger = loggerConfiguration.CreateLogger();
        }
        public void Log(CustomLogLevel logLevel, string name, string message)
        {
            var formattedMessage = $"[{name}] -- {message}";

            switch (logLevel)
            {
                case CustomLogLevel.Trace:
                    _serilogLogger.Verbose(formattedMessage);
                    break;
                case CustomLogLevel.Debug:
                    _serilogLogger.Debug(formattedMessage);
                    break;
                case CustomLogLevel.Information:
                    _serilogLogger.Information(formattedMessage);
                    break;
                case CustomLogLevel.Warning:
                    _serilogLogger.Warning(formattedMessage);
                    break;
                case CustomLogLevel.Error:
                    _serilogLogger.Error(formattedMessage);
                    break;
                case CustomLogLevel.Critical:
                    _serilogLogger.Fatal(formattedMessage);
                    break;
                default:
                    _serilogLogger.Information(formattedMessage);
                    break;
            }
        }
    }
}
