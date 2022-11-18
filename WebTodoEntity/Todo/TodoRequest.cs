using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace WebTodoEntity.Todo
{
    public static class TodoRequest
    { 
        public static WebApplication RegisterEndpoints(this WebApplication app)
        {
            app.MapGet("/todos", GetAll);
            app.MapGet("/todos/completed", GetCompleteTodos);
            app.MapGet("todos/{id}", GetTodoById);
            app.MapPut("/todos/{id}", UpdateTodo);
            app.MapPost("/todos", Create);
            app.MapDelete("/todos/{id}", DeleteTodo);

            return app;
        }


        private static async Task<IResult> GetAll(TodoDb db)
        {
            return TypedResults.Ok(await db.Todos.ToListAsync());
        }

        private static async Task<IResult> GetCompleteTodos(TodoDb db)
        {
            return TypedResults.Ok(await db.Todos.Where(n => n.IsComplete).ToListAsync());
        }

        static async Task<IResult> GetTodoById(Guid id, TodoDb db)
        {
            return await db.Todos.FindAsync(id)
                is Todo todo ?
                TypedResults.Ok(todo)
                : TypedResults.NotFound();
        }

        static async Task<IResult> Create(Todo todo, TodoDb db)
        {
            db.Todos.Add(todo);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/todoitems/{todo.Id}", todo);
        }

        static async Task<IResult> UpdateTodo([FromRoute]Guid id, [FromBody]Todo updatedTodo, TodoDb db)
        {
            var todo = db.Todos.FindAsync(id);

            if (todo.Result is null)
                return TypedResults.NotFound();

            todo.Result.IsComplete = updatedTodo.IsComplete;
            todo.Result.Description = updatedTodo.Description;

            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        static async Task<IResult> DeleteTodo([FromRoute]Guid id, TodoDb db)
        {
            if (await db.Todos.FindAsync(id) is Todo todo)
            {
                db.Todos.Remove(todo);
                await db.SaveChangesAsync();
                return TypedResults.Ok(todo);
            }

            return TypedResults.NotFound(); 
        } 
    }
}
