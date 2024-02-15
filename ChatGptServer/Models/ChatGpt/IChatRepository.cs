namespace ChatGptServer.Models.ChatGpt
{
    public interface IChatRepository
    {
        IEnumerable<Chat> Chats { get; }
        Chat this[int id] { get; }
        Chat Add(Chat newChat);
    }
}
