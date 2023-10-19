using System.Runtime.CompilerServices;
using System.Security.Principal;
using TeamGPT.Models;
using TeamGPT.Utilities;

namespace TeamGPT.Activities
{
    public enum Status
    {
        NotAssigned, // Created by not assigned
        Assigned, // Assigned to Human, but not currently being worked.
        InProgress, // Actively being worked by Human
        InReview, // Assignee completed work, and output is being reviewed by Assigner 
        Completed // Assignee completed work, and output has been approved by Assigner
    }

    public class Goal
    {
        private readonly ApplicationSettings _settings;        
        public Agent Conceiver { get; private set; }  
        public string Description { get; private set; }
        public bool? IsDecomposable { get; set; }
        public bool IsMet => (Status == Status.Completed);
        public Goal? Parent { get; private set; }
        public List<Goal> Children { get; private set; }
        public Agent? Assignee { get; private set; }
        public Status Status { get; private set; }
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

        public Goal(ApplicationSettings settings, Goal? parent, Agent conceiver, string description, double priority)
        {
            this._settings = settings;
            this.Conceiver = conceiver;
            this.Assignee = null;
            this.Parent = parent;
            this.Children = new();
            this.Description = description;
            this.Priority = priority;
            this.IsDecomposable = null; // default initial objective decomposability set to null (the assignee will decide)
            this.Status = Status.NotAssigned;
        }

        public void Decompose(List<Goal> objectives)
        {
            this.IsDecomposable = false;
            this.Children = objectives;
        }

        public void AddChild(Goal objective)
        {
            this.Children.Add(objective);
        }

        public void Assign(Agent assignee)
        {
            Assignee = assignee;
            Status = Status.Assigned;
        }

        public void Complete(string outcome)
        {
            this.Outcome = outcome;
            Status = Status.Completed;
        }

        public void Activate()
        {
            this.Status = Status.InProgress;
        }

        public string Review()
        {
            IPrincipal currentUser = Thread.CurrentPrincipal;
            
            // Confirm the reviewer has the authority to review
            if (currentUser.Identity.Name == this.Conceiver.Name)
            {
                this.Status = Status.InReview;
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