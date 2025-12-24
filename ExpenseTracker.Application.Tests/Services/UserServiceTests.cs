using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using FluentAssertions;
using Moq;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _service = new UserService(_userRepoMock.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_Should_Return_All_Users()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "a@test.com" },
            new User { Id = 2, Email = "b@test.com" }
        };

        _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _service.GetAllUsersAsync();

        //Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task GetUserByIdAsync_Should_Return_User_When_Exists()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com" };

        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByIdAsync(1);

        //Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_Should_Return_User_When_Exists()
    {
        // Arrange
        var email = "test@test.com";
        var user = new User { Id = 1, Email = email };

        _userRepoMock.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByEmailAsync(email);

        //Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task AddUserAsync_Should_Add_And_Save()
    {
        // Arrange
        var user = new User { Email = "new@test.com" };

        // Act
        await _service.AddUserAsync(user);

        //Assert
        _userRepoMock.Verify(r => r.AddAsync(user), Times.Once);
        _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_Should_Update_And_Save()
    {
        // Arrange
        var user = new User { Id = 1, Email = "update@test.com" };

        // Act
        await _service.UpdateUserAsync(user);

        //Assert
        _userRepoMock.Verify(r => r.Update(user), Times.Once);
        _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_Should_Remove_And_Save_When_User_Exists()
    {
        // Arrange
        var user = new User { Id = 1, Email = "delete@test.com" };

        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        await _service.DeleteUserAsync(1);

        //Assert
        _userRepoMock.Verify(r => r.Remove(user), Times.Once);
        _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_Should_Do_Nothing_When_User_Not_Found()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        // Act
        await _service.DeleteUserAsync(1);

        //Assert
        _userRepoMock.Verify(r => r.Remove(It.IsAny<User>()), Times.Never);
        _userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}