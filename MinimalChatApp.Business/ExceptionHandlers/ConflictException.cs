namespace MinimalChatApp.Business.Service
{
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base($"{message}.")
        {}
    }
}
