using TeamGPT.Models;
using TeamGPT.Utilities;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using OpenAI.Builders;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.SharedModels;
using OpenAI.Interfaces;
using OpenAI.Managers;
using System.ComponentModel;
using System.Text.Json;

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
            foreach (OpenAI_API.Chat.ChatMessage message in this.conversation.Messages)
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

            foreach (OpenAI_API.Chat.ChatMessage message in conversation.Messages)
            {
                thoughts.Add(toThought((message.Role, message.Content)));
            }

            return thoughts;
        }

        public async Task<Team> DefineTeamFunction(string team_directive)
        {
            // Create the new team
            Team team = new(this._settings);

            IOpenAIService sdk = bedalgoAiService;
            Console.WriteLine("Chat Function Call Testing is starting:", ConsoleColor.Cyan);

            var fn1 = new FunctionDefinitionBuilder("get_team_member", "Identify a human for a given objective")
                .AddParameter("name", PropertyDefinition.DefineString("The name of the team member"))
                .AddParameter("background", PropertyDefinition.DefineString("The background of the team member (e.g., 'Engineer', 'Artist', 'Doctor')"))
                .AddParameter("skills", PropertyDefinition.DefineArray(PropertyDefinition.DefineString("The skills of the team member (e.g., 'Programming', 'Drawing', 'Surgery')")))
                .AddParameter("knowledgedomains", PropertyDefinition.DefineArray(PropertyDefinition.DefineString("The knowledge domains of the team member (e.g., 'Machine Learning', 'Renaissance Art', 'Cardiology')")))
                .AddParameter("proclivities", PropertyDefinition.DefineArray(PropertyDefinition.DefineString("The proclivities of the team member (e.g., 'Analytical', 'Creative', 'Patient')")))
                .Validate()
                .Build();

            // Define the structure of the Human object
            var humanDefinition = PropertyDefinition.DefineObject(
                new Dictionary<string, PropertyDefinition>
                {
                    { "name", PropertyDefinition.DefineString("The name of the team member") },
                    { "background", PropertyDefinition.DefineString("The background of the team member (e.g., 'Engineer', 'Artist', 'Doctor')") },
                    { "skills", PropertyDefinition.DefineArray(PropertyDefinition.DefineString("The skills of the team member (e.g., 'Programming', 'Drawing', 'Surgery')")) },
                    { "knowledgeDomains", PropertyDefinition.DefineArray(PropertyDefinition.DefineString("The knowledge domains of the team member (e.g., 'Machine Learning', 'Renaissance Art', 'Cardiology')")) },
                    { "proclivities", PropertyDefinition.DefineArray(PropertyDefinition.DefineString("The proclivities of the team member (e.g., 'Analytical', 'Creative', 'Patient')")) }
                },
                required: new List<string> { "name", "background", "skills", "knowledgedomains", "proclivities" },
                additionalProperties: false,
                description: "Represents the attributes of a Human team member.",
                @enum: null // Provide null for enum as it's not applicable for the Human object
            );

            // Use the Human object definition as a parameter for the get_team_members function
            var fn2 = new FunctionDefinitionBuilder("get_team_members", "Create a team for a given objective")
                .AddParameter("humans", PropertyDefinition.DefineArray(humanDefinition))
                .Validate()
                .Build();

            try
            {
                Console.WriteLine("Chat Function Call Test:", ConsoleColor.DarkCyan);
                var completionResult = await sdk.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                {
                    Messages = new List<OpenAI.ObjectModels.RequestModels.ChatMessage>
                    {
                        OpenAI.ObjectModels.RequestModels.ChatMessage.FromSystem("Don't make assumptions about what values to plug into functions. Ask for clarification if a user request is ambiguous."),
                        OpenAI.ObjectModels.RequestModels.ChatMessage.FromUser($"Create the optimatel team to complete the objective: '{team_directive}'")
                    },
                    Functions = new List<FunctionDefinition> {fn1, fn2},
                    // optionally, to force a specific function:
                    FunctionCall = new Dictionary<string, string> { { "name", "get_team_members" } },
                    MaxTokens = 500,
                    Model = OpenAI.ObjectModels.Models.Gpt_4
                });

                /*  expected output along the lines of:
                
                    Message:
                    Function call:  get_n_day_weather_forecast
                    location: Chicago, USA
                    format: celsius
                    num_days: 5
                */
                if (completionResult.Successful)
                {
                    var choice = completionResult.Choices.First();
                    Console.WriteLine($"Message:        {choice.Message.Content}");

                    var fn = choice.Message.FunctionCall;
                    if (fn != null && fn.Name == "get_team_members")
                    {
                        Console.WriteLine($"Function call:  {fn.Name}");
                        var humansListData = fn.ParseArguments()["humans"];

                        if (humansListData is JsonElement humansElement && humansElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var humanData in humansElement.EnumerateArray())
                            {
                                string name = humanData.GetProperty("name").GetString();
                                string background = humanData.GetProperty("background").GetString();
                                List<string> skills = humanData.GetProperty("skills").EnumerateArray().Select(item => item.GetString()).ToList();
                                List<string> knowledgeDomains = humanData.GetProperty("knowledgeDomains").EnumerateArray().Select(item => item.GetString()).ToList();
                                List<string> proclivities = humanData.GetProperty("proclivities").EnumerateArray().Select(item => item.GetString()).ToList();

                                var persona = new Persona
                                {
                                    Background = background,
                                    Skills = skills,
                                    KnowledgeDomains = knowledgeDomains,
                                    Proclivities = proclivities
                                };
                                
                                Human human = new(this._settings, name, persona);
                                team.AddMember(human);
                            }
                        }    
                    }                
                }
                else
                {
                    if (completionResult.Error == null)
                    {
                        throw new Exception("Unknown Error");
                    }

                    Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return team;
        }

        private List<string> convertToList(object value)
        {
            List<string> strings = new();

            if (value is System.Text.Json.JsonElement jsonElement && jsonElement.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                foreach (var element in jsonElement.EnumerateArray())
                {
                    if (element.ValueKind == System.Text.Json.JsonValueKind.String)
                    {
                        strings.Add(element.GetString());
                    }
                }
            }

            return strings;
        }

        public async Task RunChatFunctionCallTest()
        {
            IOpenAIService sdk = bedalgoAiService;

            Console.WriteLine("Chat Function Call Testing is starting:", ConsoleColor.Cyan);

            // example taken from:
            // https://github.com/openai/openai-cookbook/blob/main/examples/How_to_call_functions_with_chat_models.ipynb

            var fn1 = new FunctionDefinitionBuilder("get_current_weather", "Get the current weather")
                .AddParameter("location", PropertyDefinition.DefineString("The city and state, e.g. San Francisco, CA"))
                .AddParameter("format", PropertyDefinition.DefineEnum(new List<string> {"celsius", "fahrenheit"}, "The temperature unit to use. Infer this from the users location."))
                .Validate()
                .Build();

            var fn2 = new FunctionDefinitionBuilder("get_n_day_weather_forecast", "Get an N-day weather forecast")
                .AddParameter("location", new PropertyDefinition {Type = "string", Description = "The city and state, e.g. San Francisco, CA"})
                .AddParameter("format", PropertyDefinition.DefineEnum(new List<string> {"celsius", "fahrenheit"}, "The temperature unit to use. Infer this from the users location."))
                .AddParameter("num_days", PropertyDefinition.DefineInteger("The number of days to forecast"))
                .Validate()
                .Build();
            var fn3 = new FunctionDefinitionBuilder("get_current_datetime", "Get the current date and time, e.g. 'Saturday, June 24, 2023 6:14:14 PM'")
                .Build();

            var fn4 = new FunctionDefinitionBuilder("identify_number_sequence", "Get a sequence of numbers present in the user message")
                .AddParameter("values", PropertyDefinition.DefineArray(PropertyDefinition.DefineNumber("Sequence of numbers specified by the user")))
                .Build();
            try
            {
                Console.WriteLine("Chat Function Call Test:", ConsoleColor.DarkCyan);
                var completionResult = await sdk.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                {
                    Messages = new List<OpenAI.ObjectModels.RequestModels.ChatMessage>
                    {
                        OpenAI.ObjectModels.RequestModels.ChatMessage.FromSystem("Don't make assumptions about what values to plug into functions. Ask for clarification if a user request is ambiguous."),
                        OpenAI.ObjectModels.RequestModels.ChatMessage.FromUser("Give me a weather report for Chicago, USA, for the next 5 days.")
                    },
                    Functions = new List<FunctionDefinition> {fn1, fn2, fn3, fn4},
                    // optionally, to force a specific function:
                    // FunctionCall = new Dictionary<string, string> { { "name", "get_current_weather" } },
                    MaxTokens = 50,
                    Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo
                });

                /*  expected output along the lines of:
                
                    Message:
                    Function call:  get_n_day_weather_forecast
                    location: Chicago, USA
                    format: celsius
                    num_days: 5
                */

                if (completionResult.Successful)
                {
                    var choice = completionResult.Choices.First();
                    Console.WriteLine($"Message:        {choice.Message.Content}");

                    var fn = choice.Message.FunctionCall;
                    if (fn != null)
                    {
                        Console.WriteLine($"Function call:  {fn.Name}");
                        foreach (var entry in fn.ParseArguments())
                        {
                            Console.WriteLine($"  {entry.Key}: {entry.Value}");
                        }
                    }
                }
                else
                {
                    if (completionResult.Error == null)
                    {
                        throw new Exception("Unknown Error");
                    }

                    Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}