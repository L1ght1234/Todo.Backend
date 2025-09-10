using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Todo_Backend.BLL.Constants;
using Todo_Backend.BLL.DTOs.Todos;
using Todo_Backend.DAL.Entities;
using Todo_Backend.DAL.Repositories;

namespace Todo_Backend.BLL.Commands.Todos.Create;
public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, Result<GetTodoResponse>>
{
    private readonly IMapper _mapper;
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<CreateTodoHandler> _logger;
    private readonly IValidator<CreateTodoRequest> _validator;


    public CreateTodoHandler(IMapper mapper,
        ITodoRepository todoRepository, 
        ILogger<CreateTodoHandler> logger, 
        IValidator<CreateTodoRequest> validator)
    {
        _mapper = mapper;
        _todoRepository = todoRepository;
        _logger = logger;
        _validator = validator;
    }
    public async Task<Result<GetTodoResponse>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _validator.ValidateAndThrowAsync(request.CreateTodoRequest, cancellationToken);

            var newTodo = _mapper.Map<Todo>(request.CreateTodoRequest);

            if (newTodo is null)
            {
                var errorMsg = TodoConstants.CreateTodoError;
                _logger.LogError(errorMsg);
                return Result.Fail(errorMsg);
            }

            await _todoRepository.AddAsync(newTodo, cancellationToken);

            var todoRsponse = _mapper.Map<GetTodoResponse>(newTodo);

            return Result.Ok(todoRsponse);
        }
        catch (ValidationException ex)
        {
            return Result.Fail<GetTodoResponse>(ex.Message);
        }
    }
}