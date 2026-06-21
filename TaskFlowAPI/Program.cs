using TaskFlowAPI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TaskFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// GET all tasks
app.MapGet("/tasks", async (TaskFlowDbContext db) =>
    Results.Ok(await db.Todos.ToListAsync()));

// GET single task
app.MapGet("/tasks/{id}", async (int id, TaskFlowDbContext db) =>
{
    var task = await db.Todos.FindAsync(id);
    return task is null ? Results.NotFound() : Results.Ok(task);
});

// POST add task
app.MapPost("/tasks", async (TodoItem item, TaskFlowDbContext db) =>
{
    db.Todos.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{item.Id}", item);
});

// PUT edit task
app.MapPut("/tasks/{id}", async (int id, TodoItem updated, TaskFlowDbContext db) =>
{
    var task = await db.Todos.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.Title = updated.Title;
    task.Description = updated.Description;
    task.Priority = updated.Priority;
    task.DueDate = updated.DueDate;
    task.IsCompleted = updated.IsCompleted;

    await db.SaveChangesAsync();
    return Results.Ok(task);
});

// DELETE task
app.MapDelete("/tasks/{id}", async (int id, TaskFlowDbContext db) =>
{
    var task = await db.Todos.FindAsync(id);
    if (task is null) return Results.NotFound();

    db.Todos.Remove(task);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();