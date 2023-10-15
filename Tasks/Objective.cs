using TeamGPT.Models;

namespace TeamGPT.Tasks
{
    public class Objective
    {
        public Human Conceiver { get; private set; }  
        public string Goal { get; private set; }
        public bool? IsDecomposable { get; set; }
        public bool IsAchieved { get; private set; }
        public Objective? Parent { get; private set; }
        public List<Objective> Children { get; private set; }
        public Human? Assignee { get; private set; }
        public List<Activity> Activities { get; private set; }
        public bool IsComplete => Activities.All(activity => activity.IsComplete);

        public Objective(Objective? parent, Human conceiver, string goal)
        {
            this.Conceiver = conceiver;
            this.Assignee = null;
            this.Parent = parent;
            this.Children = new();
            this.Goal = goal;
            this.IsAchieved = false;
            this.Activities = new();
            this.IsDecomposable = null; // default initial objective decomposability set to null (the assignee will decide)
        }

        public void Decompose(List<Objective> objectives)
        {
            this.IsDecomposable = false;
            this.Children = objectives;
        }

        public void AddActivity(Activity activity)
        {
            this.Activities.Add(activity);
        }
    }
}