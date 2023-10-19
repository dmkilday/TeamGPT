using System.Collections.Generic;
using System.Threading.Tasks;
using TeamGPT.Activities;
using TeamGPT.Utilities;

namespace TeamGPT.Models
{
    public class Brain : Cognition
    {
        public Persona Persona { get; private set; }

        public Brain(ApplicationSettings settings, Human owner, Persona persona) 
            : base(settings, owner)  // Calling the base constructor
        {
            this.Persona = persona;
        }

        public override async Task<string> Think(string input)
        {
            // Create the thought
            Thought inputThought = new Thought(true, input);

            // Call OpenAI to get thought response using the oai member from Cognition
            Thought outputThought = await oai.Prompt(inputThought);
            
            return outputThought.Content;
        }

        public List<Thought> GetThoughts()
        {
            return oai.GetThoughts();
        }

        public async Task<Team> DefineTeam(Goal goal)
        {
            Team team = await oai.DefineTeamFunction(goal.Description);
            return team;
        }

        public Choice Choose(Goal goal)
        {
            Choice choice = new(this._settings, goal);
            return choice;
        }

        // ... Other methods specific to the Brain ...
    }
}