using Serilog;

namespace TeamGPT.Utilities
{
    public class ErrorHandler
    {
        private readonly ApplicationSettings _settings;
        private readonly Logger _logger;

        public ErrorHandler(ApplicationSettings settings)
        {
            this._settings = settings;
            this._logger = this._settings.LoggerInstance;
        }

        public void Handle(Exception ex)
        {
            // Determine the error level
            switch (ex)
            {
                case CriticalException _:
                    _logger.Log(Logger.CustomLogLevel.Critical, "System", $"CRITICAL ERROR: {ex.Message}");
                    break;
                case WarningException _:
                    _logger.Log(Logger.CustomLogLevel.Warning, "System", $"WARNING: {ex.Message}");
                    break;
                default:
                    _logger.Log(Logger.CustomLogLevel.Error, "System", $"UNHANDLED ERROR: {ex.Message}");
                    break;
            }

            // You can extend this to decide what to do next.
        }
    }
}
    public class CriticalException : Exception
    {
        public CriticalException() { }
        public CriticalException(string message) : base(message) { }
        public CriticalException(string message, Exception inner) : base(message, inner) { }
    }

    public class WarningException : Exception
    {
        public WarningException() { }
        public WarningException(string message) : base(message) { }
        public WarningException(string message, Exception inner) : base(message, inner) { }
    }