using Microsoft.EntityFrameworkCore;
using ToDoApi;

var builder = WebApplication.CreateBuilder(args);

//Add DI - AddService 
builder.Services.AddDbContext<ToDoDb>(opt => opt.UseInMemoryDatabase("ToDoList"));

var app = builder.Build();

// Configure Pipeline - UseMethod


app.MapGet("/todoitems", (ToDoDb db) =>
{
    return db.Items.ToList();
});

app.MapGet("/todoitems/{id}", (int id, ToDoDb db) =>
{
    var item = db.Items.Find(id);
    if (item == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(item);
});

app.MapPost("/todoitems", (ToDoItem item, ToDoDb db) =>
{
    db.Items.Add(item);
    db.SaveChanges();
    return Results.Created($"/todoitems/{item.Id}", item);
});

app.MapPut("/todoitems/{id}", (int id, ToDoItem item, ToDoDb db) =>
{
    if (id != item.Id)
    {
        return Results.BadRequest();
    }
    var existingItem = db.Items.Find(id);
    if (existingItem == null)
    {
        return Results.NotFound();
    }
    existingItem.Name = item.Name;
    existingItem.IsCompleted = item.IsCompleted;
    db.SaveChanges();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", (int id, ToDoDb db) =>
{
    var item = db.Items.Find(id);
    if (item == null)
    {
        return Results.NotFound();
    }
    db.Items.Remove(item);
    db.SaveChanges();
    return Results.NoContent();
});

app.Run();
