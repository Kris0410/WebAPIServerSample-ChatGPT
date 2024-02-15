using System.Text;

namespace ChatGptServer.Models.ChatGpt
{
    public class Chat
    {
        public Chat()
        {
            
        }

        public Chat(int id)
        {
            this.Id = id;
        }

        public int Id { get; set; }

        public string? Message { get; set; }

        public string? Reply { get; set; }

        public string? Error { get; set; }
    }
}
