using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Todo_Backend.BLL.Commands.Todos.Create;
using Todo_Backend.BLL.DTOs.Todos;
using Todo_Backend.DAL.Entities;
using Todo_Backend.DAL.Enums;
using Todo_Backend.DAL.Repositories;
using Xunit;

namespace Todo_Backend.Tests.Handlers.Todos;

public class CreateTodoHandlerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ITodoRepository> _todoRepositoryMock = new();
    private readonly Mock<ILogger<CreateTodoHandler>> _loggerMock = new();
    private readonly Mock<IValidator<CreateTodoRequest>> _validatorMock = new();
    private readonly CreateTodoHandler _handler;

    public CreateTodoHandlerTests()
    {
        _handler = new CreateTodoHandler(_mapperMock.Object, _todoRepositoryMock.Object, _loggerMock.Object, _validatorMock.Object);

        _todoRepositoryMock
    .Setup(r => r.AddAsync(It.IsAny<Todo>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync((Todo t, CancellationToken _) => t);

    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_WhenTodoIsCreated()
    {
        var createRequest = new CreateTodoRequest("Test todo", "2025-09-30", Status.Todo);
        var command = new CreateTodoCommand(createRequest);

        var todoId = Guid.NewGuid();
        var todoEntity = new Todo { Id = todoId, Title = "Test todo", Deadline = "2025-09-30", Status = Status.Todo };
        var response = new GetTodoResponse(todoId, "Test todo", "2025-09-30", Status.Todo);

        _mapperMock.Setup(m => m.Map<Todo>(createRequest)).Returns(todoEntity);
        _mapperMock.Setup(m => m.Map<GetTodoResponse>(todoEntity)).Returns(response);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(todoId, result.Value.id);
        Assert.Equal("Test todo", result.Value.Title);
        Assert.Equal("2025-09-30", result.Value.Deadline);
        Assert.Equal(Status.Todo, result.Value.Status);

        _todoRepositoryMock.Verify(r => r.AddAsync(todoEntity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenMapperReturnsNull()
    {
        var createRequest = new CreateTodoRequest("Bad todo", null, Status.Todo);
        var command = new CreateTodoCommand(createRequest);

        _mapperMock.Setup(m => m.Map<Todo>(createRequest)).Returns((Todo?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains(result.Errors, e => e.Message == "Can not create todo");
        _todoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Todo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectTodo()
    {
        var createRequest = new CreateTodoRequest("Repo Test", null, Status.InProgress);
        var command = new CreateTodoCommand(createRequest);

        var todoEntity = new Todo { Id = Guid.NewGuid(), Title = "Repo Test", Deadline = null, Status = Status.InProgress };
        var response = new GetTodoResponse(todoEntity.Id, todoEntity.Title, todoEntity.Deadline, todoEntity.Status);

        _mapperMock.Setup(m => m.Map<Todo>(createRequest)).Returns(todoEntity);
        _mapperMock.Setup(m => m.Map<GetTodoResponse>(todoEntity)).Returns(response);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _todoRepositoryMock.Verify(
            r => r.AddAsync(
                It.Is<Todo>(t => t.Title == "Repo Test" && t.Deadline == null && t.Status == Status.InProgress),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}