using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();

            var inMemorySettings = new Dictionary<string, string?>
            {
                { "Jwt:Key", "SuperSecretKeyForJwtAuthentication123!!" },
                { "Jwt:Issuer", "MyAppIssuer" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _userController = new UserController(_userServiceMock.Object, configuration);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOkResult_WithUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new() { Id = 1, Name = "John Doe", LastName = "Smith", Username = "johndoe", Email = "john@example.com" }
            };
            _userServiceMock.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _userController.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            
            Assert.NotNull(response);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnOk_WhenUserIsRegistered()
        {
            // Arrange
            var newUser = new User
            {
                Id = 1,
                Name = "Test",
                LastName = "User",
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpass"
            };

            _userServiceMock
                .Setup(service => service.GetUserByEmailAsync(newUser.Email))
                .ReturnsAsync((User?)null);

            _userServiceMock
                .Setup(service => service.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userController.Register(newUser);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var response = actionResult.Value;
            
            var message = response?.GetType().GetProperty("message")?.GetValue(response);
            Assert.NotNull(message);
            Assert.Equal("User registered successfully!", message);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            // Arrange
            var existingUser = new User
            {
                Id = 1,
                Name = "Existing User",
                LastName = "Test",
                Username = "existinguser",
                Email = "existing@example.com",
                PasswordHash = "hashedpass"
            };

            _userServiceMock
                .Setup(service => service.GetUserByEmailAsync(existingUser.Email))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _userController.Register(existingUser);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value;

            var message = response?.GetType().GetProperty("message")?.GetValue(response);
            Assert.NotNull(message);
            Assert.Equal("Email is already in use.", message);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenValidCredentials()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "john@example.com", PasswordHash = "password123" };
            var user = new User
            {
                Id = 1,
                Name = "John Doe",
                LastName = "Smith",
                Username = "johndoe",
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
            };

            _userServiceMock
                .Setup(service => service.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _userController.Login(loginDto);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var response = actionResult.Value;

            var token = response?.GetType().GetProperty("token")?.GetValue(response);
            Assert.NotNull(token);
            Assert.False(string.IsNullOrEmpty(token?.ToString()));
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "wrong@example.com", PasswordHash = "password123" };

            _userServiceMock
                .Setup(service => service.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userController.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = unauthorizedResult.Value;

            var message = response?.GetType().GetProperty("message")?.GetValue(response);
            Assert.NotNull(message);
            Assert.Equal("User not found", message);
        }
    }
}
