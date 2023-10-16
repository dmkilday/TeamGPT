using System.Net.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Serilog;

using TeamGPT.Models;
using TeamGPT.Utilities;
using TeamGPT.Tasks;
using TeamGPT.Services;

namespace TeamGPT
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // Create a new service collection & configuration
            var appSettings = Configurator.ConfigureApplication();
            Console.WriteLine($"App Name: {appSettings.ApplicationName}");
            Console.WriteLine($"Version: {appSettings.Version}");
            Console.WriteLine($"Default Directive: {appSettings.DefaultDirective}");

            // Get the main directive from the user
            var default_directive = appSettings.DefaultDirective;
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

            var solutionArchitect = new Persona
            {
                Background = "Solution Architect",
                Skills = new List<string> { "Software Engineering", "Software Design", "Enterprise Architecture", "Solution Architecture" },
                KnowledgeDomains = new List<string> { "Development Languages", "Architecture Frameworks", "Waterfall Methodology", "Agile Methdology" },
                Proclivities = new List<string> { "Ideation", "Problem Solving", "Vision", "Empathy" }
            };            

            // Create the team
            Human me = new(appSettings, "Damian", solutionArchitect);
            TeamBuilder tb = new(appSettings, me);
            Objective main_objective = new Objective(appSettings, null, conceiver: me, goal: main_directive);
            Team team = await tb.Build(main_objective); // Create a new team based on the main_objective

            //Team team = new Team(appSettings);
            // var alice = new Human(appSettings, "Alice", engineer);
            // alice.JoinTeam(team);
            // var bob = new Human(appSettings, "Bob", artist);
            // bob.JoinTeam(team);

            // Display team
            Console.WriteLine();
            Console.WriteLine("Team Composition:");
            foreach (Human member in team.Members)
            {
                Console.WriteLine(member);
            }

            // Create objective & assign to Bob
            Objective objective = new Objective(appSettings, null, conceiver: me, goal: main_directive);
            Human assignee = team.Members[0];
            me.Assign(objective, assignee);
            
            // Output the task outcome
            if (assignee.Objective.IsComplete)
            {
                Console.WriteLine($"{assignee.Name} has finished the Objective '{objective.Goal}'.");
                foreach (Activity activity in assignee.Objective.Activities)
                {
                    Console.WriteLine($"Activity Outcome: {activity.Outcome}");
                }
            }

            // Cleanup the log
            Log.CloseAndFlush();
        }
    }
}