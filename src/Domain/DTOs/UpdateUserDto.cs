namespace Domain.DTOs
{
    public class UpdateUserDto
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
    }
}
