using TeamGPT.Services;
using TeamGPT.Utilities;

namespace TeamGPT.Models
{
    public abstract class Cognition
    {
        protected readonly ApplicationSettings _settings;
        protected OAI oai;
        public Agent Owner { get; protected set; }

        protected Cognition(ApplicationSettings settings, Agent owner)
        {
            this._settings = settings;
            this.Owner = owner;
            oai = new OAI(_settings, this);
        }

        public abstract Task<string> Think(string prompt);

        // Add any other abstract methods or properties that will be common to all derived Cognition classes
    }
}
