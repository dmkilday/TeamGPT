namespace TeamGPT.Utilities
{
    public class ApplicationSettings
    {
        public string ApplicationName { get; set; }
        public string Version { get; set; }
        public string ApiKey { get; set; }
        public string DefaultDirective { get; set; }
        public Logging Logging { get; set; }

        private readonly Logger _logger;

        public Logger LoggerInstance => _logger;

        public ApplicationSettings(Logger logger)
        {
            _logger = logger;
        }
    }

    public class Logging
    {
        public LogLevelSettings LogLevel { get; set; }

        public class LogLevelSettings
        {
            public string Default { get; set; }
            public string ErrorHandler { get; set; }
        }
    }
}