using TeamGPT.Models;
using TeamGPT.Activities;
using TeamGPT.Utilities;

namespace TeamGPT.Services
{
    public class TeamBuilder
    {
        private ApplicationSettings _settings;
        private Human _manager;
        public List<Human> Humans { get; private set; }

        public TeamBuilder(ApplicationSettings settings, Human manager)
        {
            this._settings = settings;
            this._manager = manager;
            this.Humans = new List<Human>();
        }

        public async Task<Team> Build(Goal goal)
        {
            Team team = await this._manager.DefineTeam(goal);
            return team;
        }
    }
}