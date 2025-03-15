namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; } 
    }
}
