namespace TeamGPT.Utilities
{
    public class ErrorHandler
    {
        private readonly Logger _logger;

        public ErrorHandler(Logger logger)
        {
            _logger = logger;
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


