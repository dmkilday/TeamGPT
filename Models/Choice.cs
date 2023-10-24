using System.Dynamic;
using TeamGPT.Activities;
using TeamGPT.Utilities;

namespace TeamGPT.Models
{
    public class Choice
    {
        private ApplicationSettings _settings;
        public ChoiceType Type { get; private set ;}
        public enum ChoiceType
        {
            Do,
            Decompose,
            Delegate,
            Collaborate,
            Prepare
        }

        public Choice(ApplicationSettings settings, Goal goal, ChoiceType type)
        {
            this._settings = settings;
            Type = type;
            
        }
    }
}