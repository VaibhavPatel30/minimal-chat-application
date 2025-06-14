namespace MinimalChatApp.Business.ExceptionHandlers
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base($"{message}.") { }
    }
}
