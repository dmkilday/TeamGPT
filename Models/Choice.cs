using TeamGPT.Tasks;
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

        public Choice(ApplicationSettings settings, Objective objective)
        {
            this._settings = settings;
            
        }
    }
}