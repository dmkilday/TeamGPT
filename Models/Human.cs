using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamGPT.Activities;
using TeamGPT.Utilities;

namespace TeamGPT.Models
{
    public class Human : Agent
    {
        private Brain Brain => Cognition as Brain;  // This is the human's cognitive interface, allowing for complex thought.
        public Team Team { get; private set; }

        public Human(ApplicationSettings settings, string name, Persona persona)
            : base(settings, name)  // Call the base constructor of Agent class
        {
            // Initialize the Brain, which will be the human's thinking mechanism
            this.Cognition = new Brain(settings, this, persona);
            this.Goals = new List<Goal>();
            
            _logger.Log(Logger.CustomLogLevel.Information, Name, "I'm alive and reporting for duty!");
        }

        // Human without a persona (will set the persona later)
        public Human(ApplicationSettings settings)
            : base(settings)  // Call the base constructor of Agent class
        {
            // Initialize the Brain, which will be the human's thinking mechanism
            this.Cognition = new Brain(settings, this);
            this.Goals = new List<Goal>();
        }

        // Retrieve the human's persona
        public Persona? GetPersona()
        {
            return this.Brain.Persona;
        }

        // Assign a task to another human
        public void Assign(Goal goal, Human assignee)
        {
            assignee.ReceiveAssignment(goal);
            assignee.Work();
        }

       // Receive a task and process it
        public override void ReceiveAssignment(Goal goal)
        {
            this.Goals.Add(goal);
            goal.Assign(this);
            this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"I have received the objective '{goal.Description}'");

            // Determine if the task can be decomposed into smaller tasks
            // goal.IsDecomposable = this.DetermineDecomposability(goal);
            // if (goal.IsDecomposable != null)
            // {
            //     if (goal.IsDecomposable == true)
            //     {
            //         this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"I am decomposing the objective '{goal.Description}'");

            //         List<Goal> sub_goals = this.Decompose(goal);

            //         foreach (Goal sub_goal in sub_goals)
            //         {
            //             this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"For objective '{goal.Description}', I created sub-objective '{sub_goal.Description}'");
            //             Human assignee = GetAssignee(sub_goal);
            //             this.Assign(sub_goal, assignee);
            //             this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"Assigned sub-objective '{sub_goal.Description}' to {assignee.Name}");
            //         }
            //     }
            // }
        }

        public override void Choose(Goal goal)
        {
            // Call the Choose Function within the brain.
            this.Brain.Choose(goal);
        }

        // Start working on the objective
        private void Work()
        {
            // Grab the next goal from my to-do list
            // and distinguish it as my CurrentGoal
            this.CurrentGoal = base.Pick();

            // Decide what to with the work (e.g. Do, Decompose, Delegate, Collaborate, Prepare / gather info...)
            // insert call to inherited Choose method here.
            this.Choose(this.CurrentGoal);

            // If the goal is legit and incomplete, do it!
            if (this.CurrentGoal != null)
            {
                if (!this.CurrentGoal.IsMet)
                {
                    this.Do(CurrentGoal);
                }
            }
        }
        // Think about a given input using the human's Brain
        public override Task<string> Think(string input)
        {
            return this.Brain.Think(input);
        }

        // Randomly get a human from the team to assign a task to
        public Human GetAssignee(Goal goal)
        {
            Random random = new Random();
            int maxMemberIndex = this.Team.Members.Count - 1;
            int randomIndex = random.Next(0, maxMemberIndex);
            Human assignee = Team.Members[randomIndex];
            return assignee;
        }

        public async Task<Team> DefineTeam(Goal goal)
        {
            Team team = await this.Brain.DefineTeam(goal);
            return team;
        }

        public async Task<Human> DefineTeamMember(Goal goal)
        {
            Human human = await this.Brain.DefineTeamMember(goal);
            return human;
        }

        // Join a team
        public void JoinTeam(Team team)
        {
            this.Team = team;
            this.Team.AddMember(this);
        }

        // Leave a team
        public void LeaveTeam(Team team)
        {
            this.Team.RemoveMember(this);
            this.Team = null;
        }

        public override string ToString()
        {
            string background = this.Brain.Persona.Background == null ? "" : string.Join(", ", this.Brain.Persona.Background);
            string proclivities = this.Brain.Persona.Proclivities == null ? "" : string.Join(", ", this.Brain.Persona.Proclivities);
            string knowlegdeDomains = this.Brain.Persona.KnowledgeDomains == null ? "" : string.Join(", ", this.Brain.Persona.KnowledgeDomains);
            string skills = this.Brain.Persona.Skills == null ? "" : string.Join(", ", this.Brain.Persona.Skills);
            return $"{this.Name}:\n" +
                    $"\tBackground: {background}\n" +
                    $"\tProclivities: {proclivities}\n" +
                    $"\tKnowledge Domains: {knowlegdeDomains}\n" +
                    $"\tSkills: {skills}";
        }
    }
}