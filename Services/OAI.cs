using TeamGPT.Models;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace TeamGPT.Services
{
    public class OAI
    {
        private string ApiKey;
        private Brain parentBrain;
        public List<(string role, string content)> Dialog { get; private set; }
        private OpenAIAPI openAiService;
        private Conversation conversation;

        public OAI(Brain brain, string apiKey)
        {
            this.parentBrain = brain;    
            this.ApiKey = apiKey;
            openAiService = new OpenAIAPI(ApiKey);
            conversation = openAiService.Chat.CreateConversation();
            conversation.Model = Model.GPT4; 

        }

        public async Task<Thought> Prompt(Thought thought)
        {
            // Set the persona in the dialog if this is the first time being prompted
            if (conversation.Messages.Count == 0)
            {
                conversation.AppendSystemMessage($"You are my brain, and I am going to have an internal conversation with you which represents my train of thought. Here is my persona which you shall emulate. Persona: {parentBrain.Owner}");
            }

            // Get the response from OpenAI API
            conversation.AppendUserInput(thought.Content);
            string response = conversation.GetResponseFromChatbotAsync().Result;
            Thought thoughtResponse = new Thought(false, response); // Create the thought response
            return thoughtResponse;
        }

        private Thought toThought((string role, string content) messageTuple)
        {
            bool isInput = false;

            if (messageTuple.role == "user")
            {
                isInput = true;
            }

            Thought thought = new Thought(isInput, messageTuple.content);

            return thought;
        }

        public List<Thought> GetThoughts()
        {
            List<Thought> thoughts = new();

            foreach (ChatMessage message in conversation.Messages)
            {
                thoughts.Add(toThought((message.Role, message.Content)));
            }

            return thoughts;
        }
    }
}