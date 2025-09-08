using Todo_Backend.DAL.Enums;

namespace Todo_Backend.DAL.Entities;
public class Todo
{
    public Guid Id { get; set; }
    public string Title { get; set; } 
    public string? Deadline { get; set; }
    public Status Status { get; set; }
}