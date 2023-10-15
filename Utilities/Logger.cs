using Microsoft.Extensions.Logging;

namespace TeamGPT.Utilities
{
    public class Logger
    {
        private readonly ApplicationSettings _settings;
        private readonly ILogger _logger;

        public Logger(ApplicationSettings settings, ILogger<Logger> logger)
        {
            this._settings = settings;
            this._logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogError(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}

