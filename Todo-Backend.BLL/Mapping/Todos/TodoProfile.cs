using AutoMapper;
using Todo_Backend.BLL.DTOs.Todos;
using Todo_Backend.DAL.Entities;

namespace Todo_Backend.BLL.Mapping.Todos;
public class TodoProfile : Profile
{
    public TodoProfile()
    {
        CreateMap<CreateTodoRequest, Todo>()
            .ReverseMap();

        CreateMap<GetTodoResponse, Todo>()
            .ReverseMap();
    }
}