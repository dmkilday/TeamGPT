using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TeamGPT.Utilities;
using TeamGPT.Tasks;

namespace TeamGPT.Models
{
    public class Human
    {
        private readonly ApplicationSettings _settings;
        public string Name { get; private set; } 
        private Brain Brain;
        public Team Team { get; private set; }
        public Objective? Objective { get; private set; }
        public Activity? CurrentActivity { get; private set; }

        public Human(ApplicationSettings settings, string name, Persona persona, Team team)
        {
            this._settings = settings;
            this.Brain = new(_settings, this, persona);
            this.Name = name;
            this.Team = team;
            this.Team.AddMember(this); // Add me to the team!
        }

        public Persona? GetPersona()
        {
            return this.Brain.Persona;
        }

        public void Assign(Objective objective, Human assignee)
        {
            assignee.ReceiveAssignment(objective);
        }

        public Human GetAssignee(Objective objective)
        {
            // TODO: Get assignee from chat GPT.
            Random random = new Random();
            int maxMemberIndex = this.Team.Members.Count - 1;
            int randomIndex = random.Next(0, maxMemberIndex);  // Randomly selects a member index
            Human assignee = Team.Members[randomIndex];
            return assignee;
        }

        public void ReceiveAssignment(Objective objective)
        {
            this.Objective = objective;
            this.Objective.IsDecomposable = this.DetermineDecomposability(objective);
            if (this.Objective.IsDecomposable != null)
            {
                // If my objective is atomic, then identify activities and start working on them
                if (this.Objective.IsDecomposable == false)
                {
                    // Determine activities and add to objective
                    this.IdentifyActivities(objective);

                    // Now that I have my activities I can start working
                    this.Work();
                }
                // If objective is decomposable, get the sub objectives and assign them
                else
                {
                    // Generate sub objectives
                    List<Objective> sub_objectives = this.Decompose(objective);

                    // Assign the sub objectives
                    foreach (Objective sub_objective in sub_objectives)
                    {
                        // Determine who the assignee should be
                        Human assignee = GetAssignee(sub_objective);

                        // Assign the objective
                        this.Assign(sub_objective, assignee);
                    }
                }
            }
        }

        public bool? DetermineDecomposability(Objective objective)
        {
            return false;
        }

        public List<Objective> Decompose(Objective objective)
        {
            // Create empty list of objectives
            List<Objective> sub_objectives = new();
            
            // Stub out the list for now
            // TODO: Call ChatGPT to get list.
            sub_objectives.Add(new Objective(this._settings, objective, this, "Sub Objective #1"));
            sub_objectives.Add(new Objective(this._settings, objective, this, "Sub Objective #2"));
            sub_objectives.Add(new Objective(this._settings, objective, this, "Sub Objective #3"));
            return sub_objectives;
        }

        public void IdentifyActivities(Objective objective)
        {
            // Stub out the activity for now
            // TODO: Call ChatGPT to get list.
            objective.AddActivity(new Activity(this._settings, objective, this, objective.Goal));
        }

        // Initiate process to do the activities for my current objective
        private void Work()
        {
            if (this.Objective != null)
            {
                if(!this.Objective.IsComplete)
                {
                    foreach(Activity activity in this.Objective.Activities)
                    {
                        // Only work on incomplete activities
                        if (!activity.IsComplete)
                        {
                            this.Do(activity);
                        }
                    }
                }
            }
        }

        private void Do(Activity activity)
        {
            // Set the current activity I'm working on
            this.CurrentActivity = activity;

            // Query my brain
            string response = this.Brain.Think(activity.Description).Result;

            // Set outcome of activity
            activity.Complete(response);

            // Clear current activity
            this.CurrentActivity = null;            
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
