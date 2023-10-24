using System.Reflection.Metadata.Ecma335;
using TeamGPT.Activities;

namespace TeamGPT.Services
{
    public class FunctionDefinition
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public object SourceObject { get; private set; }
        public object TargetObject { get; private set; }
        public FunctionType Type { get; private set; }       
        public enum FunctionType
        {
            Do,
            Decompose,
            Delegate,
            Collaborate,
            Prepare
        } 

        public FunctionDefinition(string name, string description, FunctionType type, object sourceObject, object targetObject)
        {
            Name = name;            
            Description = description;
            Type = type;
            SourceObject = sourceObject;
            TargetObject = targetObject;
        }        
    }
}
