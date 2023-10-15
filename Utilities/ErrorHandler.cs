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
            // Log the exception
            _logger.LogError(ex);

            // You can extend this to decide what to do next.
            // E.g., you might want to re-throw, or gracefully shut down parts of your application, etc.
        }
    }
}


