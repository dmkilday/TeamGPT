using System.Runtime.CompilerServices;
using System.Security.Principal;
using TeamGPT.Models;
using TeamGPT.Utilities;

namespace TeamGPT.Tasks
{
    public enum ObjectiveStatus
    {
        NotAssigned, // Created by not assigned
        Assigned, // Assigned to Human, but not currently being worked.
        InProgress, // Actively being worked by Human
        InReview, // Assignee completed work, and output is being reviewed by Assigner 
        Completed // Assignee completed work, and output has been approved by Assigner
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
        private double priority;

        public double Priority // Only allows values between 0 and 1
        {
            get { return priority; }
            private set
            {
                if (value >= 0 && value <= 1)
                {
                    priority = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Priority must be between 0 and 1.");
                }
            }
        }

        public Objective(ApplicationSettings settings, Objective? parent, Human conceiver, string goal, double priority)
        {
            this._settings = settings;
            this.Conceiver = conceiver;
            this.Assignee = null;
            this.Parent = parent;
            this.Children = new();
            this.Goal = goal;
            this.Priority = priority;
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

        public string Review()
        {
            IPrincipal currentUser = Thread.CurrentPrincipal;
            
            // Confirm the reviewer has the authority to review
            if (currentUser.Identity.Name == this.Conceiver.Name)
            {
                this.Status = ObjectiveStatus.InReview;
                return this.Outcome;
            }
            else
            {
                throw new UnauthorizedAccessException("Only the Conceiver can review this objective.");
            }

            return this.Outcome;
        }                   
    }
}