using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UserRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new DatabaseContext(options);
        _userRepository = new UserRepository(_context);
    }

    [Fact]
    public async Task AddUser_ShouldSaveUser()
    {
        // Arrange
        var user = new User 
        { 
            Name = "Test", 
            LastName = "User",
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };

        // Act
        await _userRepository.AddUserAsync(user);
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");

        // Assert
        Assert.NotNull(savedUser);
        Assert.Equal(user.Email, savedUser.Email);
        Assert.NotNull(savedUser.PasswordHash);
    }
}
