using System.ComponentModel;
using TeamGPT.Services;
using TeamGPT.Tasks;
using TeamGPT.Utilities;

namespace TeamGPT.Models
{
    public class Brain
    {
        private readonly ApplicationSettings _settings;
        public Human Owner { get; private set; }
        public Persona Persona { get; private set; }
        private OAI oai;

        public Brain(ApplicationSettings settings, Human owner, Persona persona)
        {
            this._settings = settings;
            this.Owner = owner;
            this.Persona = persona;
            oai = new(_settings, this);
        }

        public async Task<string> Think(string input)
        {
            // Create the thought
            Thought inputThought = new Thought(true, input);

            // Call OpenAI to get thought response
            Thought outputThought = await oai.Prompt(inputThought);
            
            return outputThought.Content;
        }

        public List<Thought> GetThoughts()
        {
            return oai.GetThoughts();
        }

        public async Task<Team> DefineTeam(Objective objective)
        {
            Team team = await oai.DefineTeamFunction(objective.Goal);
            return team;
        }

        public Choice Choose(Objective objective)
        {
            Choice choice = new(this._settings, objective);

            return choice;
        }
    }
}