namespace TeamGPT.Models
{
    public class Activity
    {
        public Objective ParentObjective { get; private set; }
        public Human Doer { get; private set; }
        public string Description { get; private set; }
        public bool IsComplete { get; private set; }
        public string? Outcome { get; private set; }

        public Activity(Objective objective, Human doer, string description)
        {
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
