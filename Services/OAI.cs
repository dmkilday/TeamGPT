using TeamGPT.Models;
using TeamGPT.Utilities;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

namespace TeamGPT.Services
{
    public class OAI
    {
        private readonly ApplicationSettings _settings;
        private string ApiKey;
        private Brain parentBrain;
        public List<(string role, string content)> Dialog { get; private set; }
        private OpenAIAPI openAiService;
        private OpenAI.Managers.OpenAIService bedalgoAiService;
        private Conversation conversation;

        public OAI(ApplicationSettings settings, Brain brain)
        {
            this._settings = settings;
            this.parentBrain = brain;
            this.ApiKey = settings.ApiKey;
            this.openAiService = new OpenAIAPI(ApiKey);
            this.conversation = openAiService.Chat.CreateConversation();
            this.conversation.Model = Model.GPT4; 

            // Set the Bedalgo OpenAI service
            this.bedalgoAiService = new OpenAI.Managers.OpenAIService(new OpenAI.OpenAiOptions()
            {
                ApiKey =  this.ApiKey
            });
        }

        public async Task<Thought> Prompt(Thought inThought)
        {
            // Set the persona in the dialog if this is the first time being prompted
            if (this.conversation.Messages.Count == 0)
            {
                this.conversation.AppendSystemMessage($"You are my brain, and I am going to have an internal conversation with you which represents my train of thought. Here is my persona which you shall emulate. Persona: {parentBrain.Owner}");
            }

            // Get the response from OpenAI API
            this.conversation.AppendUserInput(inThought.Content);

            //Thought outThought = ChatCompletion(inThought);
            Thought outThought = await ChatCompletionEnhanced(inThought);

            return outThought;
        }

        // ChatCompletion for standard OpenAI API adapter (does not support function calls)
        public Thought ChatCompletion(Thought inThought)
        {
            Thought outThought;

            // Call OpenAI standard .NET adapter
            string response = this.conversation.GetResponseFromChatbotAsync().Result;
            outThought = new Thought(false, response); // Create the thought response
            return outThought;
        }

        // ChatCompletion for Betalgo OpenAI API adapter (supports function calls)
        public async Task<Thought> ChatCompletionEnhanced(Thought inThought)
        {
            Thought outThought;
            outThought = await PromptBetalgo(inThought);
            return outThought;
        }        

        public async Task<Thought> PromptBetalgo(Thought inThought)
        {
            Thought outThought;
            
            // Get conversation messages and convert to Betalgo ChatMessage list.
            List<OpenAI.ObjectModels.RequestModels.ChatMessage> messages = new();
            foreach (ChatMessage message in this.conversation.Messages)
            {
                messages.Add(new OpenAI.ObjectModels.RequestModels.ChatMessage(message.Role, message.Content));
            }

            var completionResult = this.bedalgoAiService.ChatCompletion.CreateCompletion(new OpenAI.ObjectModels.RequestModels.ChatCompletionCreateRequest
            {
                Messages = messages,
                Model = OpenAI.ObjectModels.Models.ChatGpt3_5Turbo
            }).Result;
            if (completionResult.Successful)
            {
                string content = completionResult.Choices.First().Message.Content;
                outThought = new Thought(false, content);
            }
            else
            {
                if (completionResult.Error == null)
                {
                    throw new Exception("Unknown Error");
                }
                outThought = new Thought(false, "Brain malfunction...unable to process request.");
                Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
            }

            return outThought;
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