using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using OpenAI.ObjectModels.RequestModels;
using TeamGPT.Activities;
using TeamGPT.Services;
using TeamGPT.Utilities;

namespace TeamGPT.Models
{
    public class Brain : Cognition
    {
        public Persona Persona { get; private set; }

        public enum DecisionOptions
        {
            DoWorkThemselves,
            GatherInformationThenWork,
            CollaborateWithOthers,
            DecomposeIntoSubGoals,
            Delegate
        }

        public enum ChoiceType
        {
            Do,
            Decompose,
            Delegate,
            Collaborate,
            Prepare
        }        

        public Brain(ApplicationSettings settings, Human owner, Persona persona) 
            : base(settings, owner)  // Calling the base constructor
        {
            this.Persona = persona;
        }

        public Brain(ApplicationSettings settings, Human owner) 
            : base(settings, owner)  // Calling the base constructor
        {
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

        public async Task<Human> DefineTeamMember(Goal goal)
        {
            Human human = await oai.DefineTeamMemberFunction(goal.Description);
            return human;
        }

        public async void Choose(Goal goal)
        {
            List<Services.FunctionDefinition> functionDefinitions = new();

            // Build get_team_members function
            // Human human = new(_settings);
            // string f1Name = "get_team_members";
            // string f1Description = "Create a team member for a given objective";
            // Services.FunctionDefinition fd1 = new(f1Name, f1Description, goal, human);
            // functionDefinitions.Add(fd1);

            // Build Do function
            string f1Name = "query_brain";
            string f1Description = "Query the brain for the information requested.";
            Services.FunctionDefinition fd1 = new(f1Name, f1Description, Services.FunctionDefinition.FunctionType.Do, goal, goal);
            functionDefinitions.Add(fd1);

            // Decompose goal into sub-goals
            string f2Name = "decompose_goal";
            string f2Description = "Decompose a goal into sub-goals.";
            Services.FunctionDefinition fd2 = new(f2Name, f2Description, Services.FunctionDefinition.FunctionType.Decompose, sourceObject: goal, targetObject: null);
            functionDefinitions.Add(fd2);

            // Have OAI figure out what function to call and populate the TargetObject propert with the data 
            await oai.ChooseFunction(functionDefinitions, goal.Description);
        }

        public void SetPersona(Persona persona)
        {
            this.Persona = persona;
        }
    }
}