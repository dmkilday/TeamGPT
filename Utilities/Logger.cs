using Microsoft.Extensions.Logging;

namespace TeamGPT.Utilities
{
    public class Logger
    {
        private readonly ILogger _logger;

        public Logger(ILogger<Logger> logger)
        {
            _logger = logger;
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

