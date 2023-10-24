using System.Net.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Serilog;

using TeamGPT.Models;
using TeamGPT.Utilities;
using TeamGPT.Activities;
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

            // Handle command line input
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
            Console.WriteLine("Building optimal team for the directive provided...");
            Human me = new(appSettings, "Damian", solutionArchitect);
            // TeamBuilder tb = new(appSettings, me);
            // Goal main_goal = new Goal(appSettings, null, conceiver: me, description: main_directive, 0.5);
            // Team team = await tb.Build(main_goal); // Create a new team based on the main_objective

            // Display team
            Console.WriteLine();
            Console.WriteLine("Team Composition:");
            // foreach (Human member in team.Members)
            // {
            //     Console.WriteLine(member);
            // }

            // Create objective & default assignment to 1st team member in the list
            Goal goal = new(appSettings, null, conceiver: me, description: main_directive, 0.5);
            Human assignee = new(appSettings, "John Assignee", engineer);
            //Human assignee = await me.DefineTeamMember(goal);
            Console.WriteLine(assignee);
            //Human assignee = team.Members[0];
            me.Assign(goal, assignee);
            
            
            // Cleanup the log
            Log.CloseAndFlush();
        }
    }
}