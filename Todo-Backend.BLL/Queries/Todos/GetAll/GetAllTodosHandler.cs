using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Todo_Backend.BLL.DTOs.Todos;
using Todo_Backend.DAL.Repositories;

namespace Todo_Backend.BLL.Queries.Todos.GetAll;
public class GetAllTodosHandler : IRequestHandler<GetAllTodosQuery, Result<List<GetTodoResponse>>>
{
    private readonly IMapper _mapper;
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<GetAllTodosHandler> _logger;

    public GetAllTodosHandler(IMapper mapper, ITodoRepository todoRepository, ILogger<GetAllTodosHandler> logger)
    {
        _mapper = mapper;
        _todoRepository = todoRepository;
        _logger = logger;
    }
    public async Task<Result<List<GetTodoResponse>>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
    {
        var todos = await _todoRepository.GetAsync(cancellationToken);
        if (todos == null)
        {
            var errorMsg = "There are no todos";
            _logger.LogError(errorMsg);
            return Result.Fail(errorMsg);
        }
        
        var todosDtos = _mapper.Map<List<GetTodoResponse>>(todos);

        return Result.Ok(todosDtos);
    }
}