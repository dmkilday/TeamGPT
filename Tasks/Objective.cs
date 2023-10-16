using System.Runtime.CompilerServices;
using TeamGPT.Models;
using TeamGPT.Utilities;

namespace TeamGPT.Tasks
{
    public enum ObjectiveStatus
    {
        NotAssigned,
        Assigned,
        InProgress,
        Completed
    }

    public class Objective
    {
        private readonly ApplicationSettings _settings;        
        public Human Conceiver { get; private set; }  
        public string Goal { get; private set; }
        public bool? IsDecomposable { get; set; }
        public bool IsAchieved => (Status == ObjectiveStatus.Completed);
        public Objective? Parent { get; private set; }
        public List<Objective> Children { get; private set; }
        public Human? Assignee { get; private set; }
        public ObjectiveStatus Status { get; private set; }
        public string Outcome { get; private set; }

        public Objective(ApplicationSettings settings, Objective? parent, Human conceiver, string goal)
        {
            this._settings = settings;
            this.Conceiver = conceiver;
            this.Assignee = null;
            this.Parent = parent;
            this.Children = new();
            this.Goal = goal;
            this.IsDecomposable = null; // default initial objective decomposability set to null (the assignee will decide)
            this.Status = ObjectiveStatus.NotAssigned;
        }

        public void Decompose(List<Objective> objectives)
        {
            this.IsDecomposable = false;
            this.Children = objectives;
        }

        public void AddChild(Objective objective)
        {
            this.Children.Add(objective);
        }

        public void Assign(Human assignee)
        {
            Assignee = assignee;
            Status = ObjectiveStatus.Assigned;
        }

        public void Complete(string outcome)
        {
            this.Outcome = outcome;
            Status = ObjectiveStatus.Completed;
        }

        public void Activate()
        {
            this.Status = ObjectiveStatus.InProgress;
        }
    }
}