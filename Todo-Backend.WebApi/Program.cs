using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Todo_Backend.BLL.Behaviors.Validation;
using Todo_Backend.BLL.Mapping.Todos;
using Todo_Backend.BLL.Queries.Todos.GetAll;
using Todo_Backend.BLL.Validators.Todos;
using Todo_Backend.DAL.Data;
using Todo_Backend.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetAllTodosHandler).Assembly);
});

builder.Services.AddAutoMapper(typeof(TodoProfile).Assembly);

builder.Services.AddScoped<ITodoRepository, TodoRepository>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo-Backend API");
    });
}

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
