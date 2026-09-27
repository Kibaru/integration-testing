using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly TodoDbContext _db;

    public TodosController(TodoDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Todo>>> GetAll()
    {
        var todos = await _db.Todos.ToListAsync();

        return Ok(todos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Todo>> GetById(int id)
    {
        var todo = await _db.Todos.FindAsync(id);

        if (todo == null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<Todo>> Create(Todo todo)
    {
        _db.Todos.Add(todo);

        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = todo.Id },
            todo);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Todo todo)
    {
        var existingTodo = await _db.Todos.FindAsync(id);

        if (existingTodo == null)
        {
            return NotFound();
        }

        existingTodo.Title = todo.Title;
        existingTodo.IsCompleted = todo.IsCompleted;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var todo = await _db.Todos.FindAsync(id);

        if (todo == null)
        {
            return NotFound();
        }

        _db.Todos.Remove(todo);

        await _db.SaveChangesAsync();

        return NoContent();
    }
}