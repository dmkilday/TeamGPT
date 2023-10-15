using TeamGPT.Utilities;

namespace TeamGPT.Models
{
    public class Team
    {
        private readonly ApplicationSettings _settings;
        public List<Human> Members { get; private set; }

        public Team(ApplicationSettings settings)
        {
            _settings = settings;
            Members = new List<Human>();
        }

        public void AddMember(Human human)
        {
            Members.Add(human);
        }
    }
}

