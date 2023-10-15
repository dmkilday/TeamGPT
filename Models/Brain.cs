using TeamGPT.Services;

namespace TeamGPT.Models
{
    public class Brain
    {
        public Human Owner { get; private set; }
        public Persona Persona { get; private set; }
        private OAI oai;

        public Brain(Human owner, Persona persona, string apiKey)
        {
            this.Owner = owner;
            this.Persona = persona;
            oai = new(this, apiKey);
        }

        public async Task<string> Think(string input)
        {
            // Create the thought
            Thought inputThought = new Thought(true, input);

            // Call OpenAI to get thought response
            Thought outputThought = await oai.Prompt(inputThought);
            
            return outputThought.Content;
        }

        public List<Thought> GetThoughts()
        {
            return oai.GetThoughts();
        }
    }
}