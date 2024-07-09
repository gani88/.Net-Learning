using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// URL Middleware to redirect if the url is tasks it will redirect to todos
app.UseRewriter(new Microsoft.AspNetCore.Rewrite.RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

// Custom Middleware
app.Use(async (context, next) => {
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started");
    await next(context);
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Completed");
});


var todos = new List<Todo>();

app.MapPost("/todos", (Todo task) => {
    todos.Add(task);
    return TypedResults.Created("/todos/{id}", task);
}).AddEndpointFilter(async (context, next) => {
    var taskArgument = context.GetArgument<Todo>(0);
    var errors = new Dictionary<string, string[]>();
    
    if (taskArgument.DueDate < DateTime.UtcNow) {
        errors.Add(nameof(Todo.DueDate), [ "Date cannot be in the past" ]);
    }
    if (taskArgument.IsCompleted) {
        errors.Add(nameof(Todo.IsCompleted), [ "Cannot add Completed todo" ]);
    }

    if (errors.Count > 0) {
        return Results.ValidationProblem(errors);
    }

    return await next(context);
});

app.MapGet("/todos", () => todos);

app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id) => {
    var targetTodo = todos.SingleOrDefault(t => id == t.Id);
    return targetTodo is null ? TypedResults.NotFound() : TypedResults.Ok(targetTodo);
});


app.MapDelete("/todos/{id}", (int id) => {
    todos.RemoveAll(t => id == t.Id);
    return TypedResults.NoContent();
});

app.Run();


public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted);