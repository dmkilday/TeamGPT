using Serilog;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using TeamGPT.Activities;
using TeamGPT.Utilities;

namespace TeamGPT.Models
{
    public abstract class Agent
    {
        public int ID { get; protected set; }
        public string Name { get; private set; }
        public List<Goal> Goals { get; protected set; }
        protected readonly ApplicationSettings _settings;
        protected readonly Logger _logger;
        protected readonly string _logFilePath;
        protected Cognition Cognition { get; private set; }
        public Activities.Goal? CurrentGoal { get; protected set; }
        public abstract void ReceiveAssignment(Goal goal);
        public abstract Task<string> Think(string input);
        public abstract override string ToString();

        public Agent(ApplicationSettings settings, string name)
        {
            this._settings = settings;
            this._logger = settings.LoggerInstance;

            // Configure a logger for each agent.
            ID = getID();
            Name = name;
            _logFilePath = $"logs/{Name}--{ID}.log";
            _logger.ConfigureLogFile(_logFilePath);       

        }

        // Get ID from AgentManager
        private int getID()
        {
            return 1;
        }

        // Assign a task to another human
        public void Assign(Goal goal, Human assignee)
        {
            assignee.ReceiveAssignment(goal);
            assignee.Work();
        }

        // Get the highest priority task available to the human
        public Goal GetWorkableGoal()
        {
            Goal goal = null;

            if (this.Goals.Count > 0)
            {
                goal = this.Goals.OrderByDescending(obj => obj.Priority).First();
                this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"I grabbed goal '{goal.Description}' from my to-do list.");
            }
            else
            {
                this._logger.Log(Logger.CustomLogLevel.Information, this.Name, "I currently have no goals in my to-do list.");
            }

            return goal;
        } 

        // Decide if an objective is decomposable into sub-objectives (placeholder)
        public bool? DetermineDecomposability(Goal goal)
        {
            return false;
        }

        // Decompose an objective into smaller sub-objectives (placeholder)
        public List<Goal> Decompose(Goal goal)
        {
            List<Goal> sub_goals = new();
            
            // sub_objectives.Add(new Objective(this._settings, objective, this, "Sub Objective #1"));
            // sub_objectives.Add(new Objective(this._settings, objective, this, "Sub Objective #2"));
            // sub_objectives.Add(new Objective(this._settings, objective, this, "Sub Objective #3"));
            return sub_goals;
        }

        private Goal Pick()
        {
            Goal nextGoal = null;

            // Get the highest priority incomplete goal from my to-do list.
            var incompleteGoals = this.Goals
                .Where(g => g.Status != Status.Completed)
                .OrderBy(g => g.Priority)
                .ToList();
            
            if (incompleteGoals != null)
                nextGoal = incompleteGoals[0];

            return nextGoal;
        }

        // Start working on the objective
        private void Work()
        {
            // Grab the next goal from my to-do list
            // and distinguish it as my CurrentGoal
            this.CurrentGoal = Pick();

            // If the goal is legit and incomplete, do it!
            if (this.CurrentGoal != null)
            {
                if (!this.CurrentGoal.IsMet)
                {
                    this.Do(CurrentGoal);
                }
            }
        }

        // Execute the objective
        private void Do(Activities.Goal goal)
        {
            goal.Activate();
            
            this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"Starting work on objective '{goal.Description}'...");
            string response = this.Think(goal.Description).Result;

            goal.Complete(response);
            this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"Completed objective '{goal.Description}'.");
            this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"Objective outcome was '{goal.Outcome}'.");

            this.CurrentGoal = null;
        }
    }
}