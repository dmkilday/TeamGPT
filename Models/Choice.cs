using TeamGPT.Activities;
using TeamGPT.Utilities;

namespace TeamGPT.Models
{
    public class Choice
    {
        private ApplicationSettings _settings;
        public enum ChoiceType
        {
            Wait,
            Prepare,
            Do
        }

        public Choice(ApplicationSettings settings, Goal goal)
        {
            this._settings = settings;
            
        }
    }
}