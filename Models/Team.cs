namespace TeamGPT.Models
{
    public class Team
    {
        public List<Human> Members { get; private set; }

        public Team()
        {
            Members = new List<Human>();
        }

        public void AddMember(Human human)
        {
            Members.Add(human);
        }
    }
}

