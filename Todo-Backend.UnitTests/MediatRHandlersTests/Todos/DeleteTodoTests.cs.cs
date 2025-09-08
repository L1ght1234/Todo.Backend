using Microsoft.Extensions.Logging;
using Moq;
using Todo_Backend.BLL.Commands.Todos.Delete;
using Todo_Backend.DAL.Repositories;
using Xunit;

namespace Todo_Backend.Tests.Handlers.Todos;

public class DeleteTodoHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepositoryMock = new();
    private readonly Mock<ILogger<DeleteTodoHandler>> _loggerMock = new();
    private readonly DeleteTodoHandler _handler;

    public DeleteTodoHandlerTests()
    {
        _handler = new DeleteTodoHandler(_todoRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenTodoIsDeleted()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var command = new DeleteTodoCommand(todoId);

        _todoRepositoryMock
            .Setup(r => r.DeleteAsync(todoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Success", result.Value);
        _todoRepositoryMock.Verify(r => r.DeleteAsync(todoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenRepositoryReturnsFalse()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var command = new DeleteTodoCommand(todoId);

        _todoRepositoryMock
            .Setup(r => r.DeleteAsync(todoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Error while deleting todo");
        _todoRepositoryMock.Verify(r => r.DeleteAsync(todoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenRepositoryReturnsFalse()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var command = new DeleteTodoCommand(todoId);

        _todoRepositoryMock
            .Setup(r => r.DeleteAsync(todoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);

        // Проверяем, что логгер был вызван с уровнем Error
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error while deleting todo")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
