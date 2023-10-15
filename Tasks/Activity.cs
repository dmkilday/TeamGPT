using TeamGPT.Utilities;
using TeamGPT.Models;

namespace TeamGPT.Tasks
{
    public class Activity
    {
        private readonly ApplicationSettings _settings;        
        public Objective ParentObjective { get; private set; }
        public Human Doer { get; private set; }
        public string Description { get; private set; }
        public bool IsComplete { get; private set; }
        public string? Outcome { get; private set; }

        public Activity(ApplicationSettings settings, Objective objective, Human doer, string description)
        {
            this._settings = settings;
            this.ParentObjective = objective;
            this.Doer = doer;
            this.Description = description;
            this.IsComplete = false;
        }

        public void Complete(string outcome)
        {
            this.Outcome = outcome;
            this.IsComplete = true;
        }
    }
}
