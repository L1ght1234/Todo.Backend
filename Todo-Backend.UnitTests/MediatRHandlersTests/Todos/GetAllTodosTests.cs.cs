using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Todo_Backend.BLL.DTOs.Todos;
using Todo_Backend.BLL.Queries.Todos.GetAll;
using Todo_Backend.DAL.Entities;
using Todo_Backend.DAL.Enums;
using Todo_Backend.DAL.Repositories;
using Xunit;

namespace Todo_Backend.Tests.Handlers.Todos;

public class GetAllTodosHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ITodoRepository> _todoRepositoryMock = new();
    private readonly Mock<ILogger<GetAllTodosHandler>> _loggerMock = new();
    private readonly GetAllTodosHandler _handler;

    public GetAllTodosHandlerTests()
    {
        _handler = new GetAllTodosHandler(_mapperMock.Object, _todoRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenTodosExist()
    {
        var todos = new List<Todo>
        {
            new Todo { Id = Guid.NewGuid(), Title = "Test 1", Status = Status.Todo },
            new Todo { Id = Guid.NewGuid(), Title = "Test 2", Status = Status.Done }
        };

        var todoResponses = new List<GetTodoResponse>
        {
            new GetTodoResponse(todos[0].Id, todos[0].Title, todos[0].Deadline, todos[0].Status),
            new GetTodoResponse(todos[1].Id, todos[1].Title, todos[1].Deadline, todos[1].Status)
        };

        _todoRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(todos);

        _mapperMock
            .Setup(m => m.Map<List<GetTodoResponse>>(todos))
            .Returns(todoResponses);

        var query = new GetAllTodosQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("Test 1", result.Value[0].Title);
        _todoRepositoryMock.Verify(r => r.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<List<GetTodoResponse>>(todos), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenTodosIsNull()
    {
        _todoRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Todo>?)null);

        var query = new GetAllTodosQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "There are no todos");
        _todoRepositoryMock.Verify(r => r.GetAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenTodosIsNull()
    {
        _todoRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Todo>?)null);

        var query = new GetAllTodosQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailed);

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("There are no todos")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
