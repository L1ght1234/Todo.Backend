using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Todo_Backend.BLL.Commands.Todos.UpdateStatus;
using Todo_Backend.BLL.DTOs.Todos;
using Todo_Backend.DAL.Entities;
using Todo_Backend.DAL.Enums;
using Todo_Backend.DAL.Repositories;
using Xunit;

namespace Todo_Backend.Tests.Handlers.Todos;

public class UpdateTodoStatusHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ITodoRepository> _todoRepositoryMock = new();
    private readonly Mock<ILogger<UpdateTodoStatusHandler>> _loggerMock = new();
    private readonly UpdateTodoStatusHandler _handler;

    public UpdateTodoStatusHandlerTests()
    {
        _handler = new UpdateTodoStatusHandler(_mapperMock.Object, _todoRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenStatusUpdated()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var updateRequest = new UpdateTodoStatusRequest(Status.Done);
        var command = new UpdateTodoStatusCommand(todoId, updateRequest);

        var updatedTodo = new Todo { Id = todoId, Title = "Test", Deadline = null, Status = Status.Done };
        var response = new GetTodoResponse(updatedTodo.Id, updatedTodo.Title, updatedTodo.Deadline, updatedTodo.Status);

        _todoRepositoryMock
            .Setup(r => r.UpdateStatusAsync(todoId, Status.Done, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTodo);

        _mapperMock
            .Setup(m => m.Map<GetTodoResponse>(updatedTodo))
            .Returns(response);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(todoId, result.Value.id);
        Assert.Equal(Status.Done, result.Value.Status);
        _todoRepositoryMock.Verify(r => r.UpdateStatusAsync(todoId, Status.Done, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenRepositoryReturnsNull()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var updateRequest = new UpdateTodoStatusRequest(Status.InProgress);
        var command = new UpdateTodoStatusCommand(todoId, updateRequest);

        _todoRepositoryMock
            .Setup(r => r.UpdateStatusAsync(todoId, Status.InProgress, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Todo?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Error while updating todo status");
        _todoRepositoryMock.Verify(r => r.UpdateStatusAsync(todoId, Status.InProgress, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenRepositoryReturnsNull()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var updateRequest = new UpdateTodoStatusRequest(Status.Todo);
        var command = new UpdateTodoStatusCommand(todoId, updateRequest);

        _todoRepositoryMock
            .Setup(r => r.UpdateStatusAsync(todoId, Status.Todo, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Todo?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error while updating todo status")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
