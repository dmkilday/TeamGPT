using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

using TeamGPT.Models;
using TeamGPT.Tasks;

namespace TeamGPT.Utilities
{
    public class Configurator
    {
        public static ApplicationSettings ConfigureApplication()
        {
            var services = new ServiceCollection();
            var configuration = BuildConfiguration();
            ConfigureLogging(services, configuration);
            ApplicationSettings appSettings = ConfigureServices(services, configuration);
            return appSettings;
        }
        
        public static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        private static void ConfigureLogging(IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/.log", rollingInterval: RollingInterval.Day) // daily log files
                .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddSerilog();
            });
        }

        private static ApplicationSettings ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add logging
            services.AddLogging(builder => 
            {
                builder.AddConsole();
            });

            // Register Logger Configuration
            services.Configure<Logger.LoggingConfiguration>(configuration.GetSection("ApplicationSettings:Logging"));

            // Register singleton for Logger
            services.AddSingleton<Logger>(sp => 
            {
                var loggingConfig = sp.GetRequiredService<IOptions<Logger.LoggingConfiguration>>().Value;
                var logger = sp.GetRequiredService<ILogger<Logger>>();
                return new Logger(logger);
            });

            services.AddLogging(builder => 
            {
                builder.AddConsole();
                builder.AddSerilog();
            });

            // Register ApplicationSettings with DI handling the instantiation
            services.AddSingleton<ApplicationSettings>(sp =>
            {
                var settings = new ApplicationSettings(sp.GetRequiredService<Logger>());
                configuration.GetSection("ApplicationSettings").Bind(settings);
                return settings;
            });

            // Register Models classes
            services.AddSingleton<Team>();
            services.AddSingleton<Human>();
            services.AddSingleton<Brain>();
            services.AddSingleton<Thought>();

            // Register Services classes
            services.AddSingleton<Services.OAI>();

            // Register Tasks classes
            services.AddSingleton<Activity>();
            services.AddSingleton<Objective>();

            // Register ErrorHandler
            services.AddSingleton<ErrorHandler>();

            // Create a service provider from the service collection & get app settings
            var serviceProvider = services.BuildServiceProvider();
            var appSettings = serviceProvider.GetRequiredService<ApplicationSettings>();

            return appSettings;
        }
    }
}