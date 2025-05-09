namespace MinimalChatApp.Entity.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserProfileDto Profile { get; set; }
    }

    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
