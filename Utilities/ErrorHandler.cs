namespace TeamGPT.Utilities
{
    public class ErrorHandler
    {
        private readonly ApplicationSettings _settings;
        private readonly Logger _logger;

        public ErrorHandler(ApplicationSettings settings, Logger logger)
        {
            this._settings = settings;
            this._logger = logger;
        }

        public void Handle(Exception ex)
        {
            // Determine the error level
            switch (ex)
            {
                case CriticalException _:
                    _logger.LogCritical(ex);
                    break;
                case WarningException _:
                    _logger.LogWarning(ex);
                    break;
                default:
                    _logger.LogError(ex);
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