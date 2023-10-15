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
            Console.WriteLine(alice);
            var bob = new Human("Bob", artist, team, apiKey);
            Console.WriteLine(bob);

            // Create objective & assign to Bob
            string goal = "Give me the best steps to create an oil painting if it's my very first painting.";
            Objective objective = new Objective(null, conceiver: alice, goal: goal);
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
