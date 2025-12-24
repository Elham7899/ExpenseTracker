using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using FluentAssertions;
using Moq;

namespace ExpenseTracker.Application.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _service = new CategoryService(_categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_Should_Return_All_Categories()
    {
        //Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food" },
            new Category { Id = 2 , Name = "Transport"}
        };

        _categoryRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        //Act
        var result = await _service.GetAllCategoriesAsync();

        //Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "Food");
    }

    [Fact]
    public async Task GetUserCategoriesAsync_Should_Return_Only_User_Categories()
    {
        // Arrange
        var userId = 10L;
        var categories = new List<Category> { new Category { Id = 1, Name = "Food", UserId = userId } };

        _categoryRepositoryMock.Setup(r => r.GetUserCategoriesAsync(userId)).ReturnsAsync(categories);

        // Act
        var result = await _service.GetUserCategoriesAsync(userId);

        // Assert
        result.Should().HaveCount(1);
        result.First().UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_Should_Return_Category_When_Exists()
    {
        //Arrange
        var category = new Category { Id = 1, Name = "Food" };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

        //Act 
        var result = await _service.GetCategoryByIdAsync(category.Id);

        //Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Food");
    }

    [Fact]
    public async Task AddCategoryAsync_Should_Add_And_Save()
    {
        //Arrange
        var category = new Category { Name = "Food" };

        //Act 
        await _service.AddCategoryAsync(category);

        //Assert
        _categoryRepositoryMock.Verify(r => r.AddAsync(category), Times.Once());
        _categoryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_Update_And_Save()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Updated" };

        // Act
        await _service.UpdateCategoryAsync(category);

        // Assert
        _categoryRepositoryMock.Verify(r => r.Update(category), Times.Once);

        _categoryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_Should_Remove_And_Save_When_Category_Exists()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Food" };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

        // Act
        await _service.DeleteCategoryAsync(1);

        // Assert
        _categoryRepositoryMock.Verify(r => r.Remove(category), Times.Once);

        _categoryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_Should_Do_Nothing_When_Category_Not_Found()
    {
        // Arrange
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category?)null);

        // Act
        await _service.DeleteCategoryAsync(1);

        // Assert
        _categoryRepositoryMock.Verify(r => r.Remove(It.IsAny<Category>()), Times.Never);

        _categoryRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

}
