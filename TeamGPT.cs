using Microsoft.Extensions.Configuration;
using TeamGPT.Models;

namespace TeamGPT
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Get configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();
            var apiKey = configuration["CustomConfig:ApiKey"];
            var default_directive = configuration["CustomConfig:DefaultDirective"];

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
            Team team = new Team();
            var alice = new Human("Alice", engineer, team, apiKey);            
            var bob = new Human("Bob", artist, team, apiKey);

            // Display team
            Console.WriteLine();
            Console.WriteLine("Team Composition:");
            foreach (Human member in team.Members)
            {
                Console.WriteLine(member);
            }

            // Create objective & assign to Bob
            Objective objective = new Objective(null, conceiver: alice, goal: main_directive);
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
    }
}
