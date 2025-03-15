using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;
using System.Linq;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepoMock.Object);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Name = "John", LastName = "Doe", Username = "johnd", Email = "john@example.com", PasswordHash = "hashed" },
            new() { Id = 2, Name = "Jane", LastName = "Doe", Username = "janed", Email = "jane@example.com", PasswordHash = "hashed" }
        };

        _userRepoMock.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUserByEmail_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User 
        { 
            Id = 1, 
            Name = "John", 
            LastName = "Doe", 
            Username = "johnd", 
            Email = "john@example.com", 
            PasswordHash = "hashed"
        };

        _userRepoMock.Setup(repo => repo.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByEmailAsync(user.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task AddUser_ShouldCallRepositoryAddUser()
    {
        // Arrange
        var newUser = new User 
        { 
            Id = 1, 
            Name = "Test", 
            LastName = "User", 
            Username = "testuser", 
            Email = "test@example.com", 
            PasswordHash = "password123" 
        };

        _userRepoMock.Setup(repo => repo.AddUserAsync(newUser)).Returns(Task.CompletedTask);

        // Act
        await _userService.AddUserAsync(newUser);

        // Assert
        _userRepoMock.Verify(repo => repo.AddUserAsync(newUser), Times.Once);
    }
}
