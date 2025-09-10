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
        var todoId = Guid.NewGuid();
        var command = new DeleteTodoCommand(todoId);

        _todoRepositoryMock
            .Setup(r => r.DeleteAsync(todoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Operation success", result.Value);
        _todoRepositoryMock.Verify(r => r.DeleteAsync(todoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenRepositoryReturnsFalse()
    {
        var todoId = Guid.NewGuid();
        var command = new DeleteTodoCommand(todoId);

        _todoRepositoryMock
            .Setup(r => r.DeleteAsync(todoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Can not delete todo");
        _todoRepositoryMock.Verify(r => r.DeleteAsync(todoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenRepositoryReturnsFalse()
    {
        var todoId = Guid.NewGuid();
        var command = new DeleteTodoCommand(todoId);

        _todoRepositoryMock
            .Setup(r => r.DeleteAsync(todoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Can not delete todo")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
