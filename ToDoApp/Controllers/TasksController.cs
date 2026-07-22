using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks([FromQuery] int? goalId, [FromQuery] int? userId, 
            [FromQuery] string? status, [FromQuery] string? due)
        {
            var query = _context.Tasks.AsQueryable();
            if (userId != null)
            {
                query = query.Where(T => T.UserId == userId);
            }
            if (goalId != null)
            {
                query = query.Where(T => T.GoalId == goalId);
            }
            if (status == "completed")
            {
                query = query.Where(T => T.CompletedDate != null);
            }
            if (status == "pending")
            {
                query = query.Where(T => T.CompletedDate == null);
            }
            if (due == "today")
            {
                query = query.Where(T => T.DueDate != null && T.DueDate.Value.Date == DateTime.UtcNow.Date);
            }
            return await query.ToListAsync();
        }

        // Fulfilled the get one task, ownership checked endpoint
        [HttpGet("{taskId}/user/{userId}")]
        public async Task<ActionResult<TaskItem>> GetTask(int taskId, int userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            if (task == null) { return NotFound(); }

            return task;
        }

        // Fulfilled the update a task's field endpoint
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem updatedTask)
        {
            if (id != updatedTask.Id) { return BadRequest(); }
            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null) { return NotFound(); }

            existingTask.Name = updatedTask.Name;
            existingTask.Description = updatedTask.Description;
            existingTask.Duration = updatedTask.Duration;
            existingTask.CompletedDate = updatedTask.CompletedDate;
            existingTask.DueDate = updatedTask.DueDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask(TaskItem newTask)
        {
            newTask.CreatedDate = DateTime.UtcNow;
            _context.Tasks.Add(newTask);
            await _context.SaveChangesAsync();

            // Writes to Activity Here
            // Placeholder (Activity Table does not exist yet)

            return CreatedAtAction(nameof(GetTask), new { taskId = newTask.Id, userId = newTask.UserId }, newTask);
        }

        // Fulfilled the toggle task completion endpoint
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> ToggleCompletion(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) { return NotFound(); }

            task.CompletedDate = task.CompletedDate == null ? DateTime.UtcNow : null;

            if (task.CompletedDate != null)
            {
                // Writes to Activity Here
                // Placeholder (Activity Table does not exist yet)
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Fulfilled the delete a task endpoint
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) { return NotFound(); }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
