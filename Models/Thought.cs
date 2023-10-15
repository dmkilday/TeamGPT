namespace TeamGPT.Models
{
    public class Thought
    {
        public bool IsInput { get; private set; }
        public string Content { get; private set; }

        public Thought(bool isInput, string content)
        {
            this.IsInput = isInput;
            this.Content = content;
        }
    }
}