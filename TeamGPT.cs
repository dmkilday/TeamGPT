using System.Net.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TeamGPT.Models;
using TeamGPT.Utilities;
using TeamGPT.Tasks;

namespace TeamGPT
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Create a new service collection
            var serviceCollection = new ServiceCollection();
            var configuration = BuildConfiguration();

            ConfigureServices(serviceCollection, configuration);

            // Create a service provider from the service collection
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Use the service provider to retrieve services and run your application logic
            var appSettings = serviceProvider.GetRequiredService<ApplicationSettings>();
            Console.WriteLine($"App Name: {appSettings.ApplicationName}");
            Console.WriteLine($"Version: {appSettings.Version}");
            Console.WriteLine($"Default Directive: {appSettings.DefaultDirective}"); 
            var default_directive = appSettings.DefaultDirective;

            // Get the main directive from the user
            string? main_directive = null;
            try
            {
                main_directive = args[0];
                Console.WriteLine($"Received main directive '{main_directive}'");
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine($"Main directive not provided. Defaulting to '{default_directive}'");
                main_directive = default_directive;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: An unhandled exception occured. Error Message: {e.Message}");
                Console.WriteLine("Exiting the application...");
                Environment.Exit(1); // Exit the application with an error code of 1
            }
                
            // Build the team
            Console.WriteLine("Building optimal team for the directive provided...");

            // Create personas
            var engineer = new Persona
            {
                Background = "Engineer",
                Skills = new List<string> { "Programming", "Mathematics" },
                KnowledgeDomains = new List<string> { "Machine Learning", "Software Development" },
                Proclivities = new List<string> { "Analytical", "Detail-oriented" }
            };

            var artist = new Persona
            {
                Background = "Artist",
                Skills = new List<string> { "Drawing", "Sculpting" },
                KnowledgeDomains = new List<string> { "Renaissance Art", "Modern Art" },
                Proclivities = new List<string> { "Creative", "Intuitive" }
            };

            // Create the team
            Team team = new Team(appSettings);
            var alice = new Human(appSettings, "Alice", engineer, team);            
            var bob = new Human(appSettings, "Bob", artist, team);

            // Display team
            Console.WriteLine();
            Console.WriteLine("Team Composition:");
            foreach (Human member in team.Members)
            {
                Console.WriteLine(member);
            }

            // Create objective & assign to Bob
            Objective objective = new Objective(appSettings, null, conceiver: alice, goal: main_directive);
            alice.Assign(objective, bob);
            
            // Output the task outcome
            if (bob.Objective.IsComplete)
            {
                Console.WriteLine($"{bob.Name} has finished the Objective '{objective.Goal}'.");
                foreach (Activity activity in bob.Objective.Activities)
                {
                    Console.WriteLine($"Activity Outcome: {activity.Outcome}");
                }
            }
        }

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
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
                return new Logger(loggingConfig, logger);
            });

            // Register ApplicationSettings with DI handling the instantiation
            services.AddSingleton<ApplicationSettings>(sp => 
            {
                var settings = new TeamGPT.Utilities.ApplicationSettings(sp.GetRequiredService<Logger>());
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
        }
    }
}