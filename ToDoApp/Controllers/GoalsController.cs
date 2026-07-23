using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GoalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GoalsController(AppDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Goal>>> GetGoals([FromQuery] int? parentGoalId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var query = _context.Goals.AsQueryable();
            query = query.Where(T => T.UserId == userId);
            if (parentGoalId != null)
            {
                query = query.Where(T => T.ParentGoalId == parentGoalId);
            } else
            {
                query = query.Where(T => T.ParentGoalId == null);
            }
            return await query.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Goal>> CreateGoal(Goal newGoal)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            newGoal.UserId = userId;
            newGoal.CreatedDate = DateTime.UtcNow;
            _context.Goals.Add(newGoal);
            await _context.SaveChangesAsync();

            // Writes to Activity Here
            // Placeholder (Activity Table does not exist yet)

            return CreatedAtAction(nameof(GetGoal), new { id = newGoal.Id }, newGoal);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Goal>> GetGoal(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null || goal.UserId != userId) { return NotFound(); }
            return goal;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoal(int id, Goal updatedGoal)
        {
            if (id != updatedGoal.Id) { return BadRequest(); }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existingGoal = await _context.Goals.FindAsync(id);
            if (existingGoal == null || existingGoal.UserId != userId) { return NotFound(); }

            existingGoal.Name = updatedGoal.Name;
            existingGoal.Description = updatedGoal.Description;
            existingGoal.TargetDate = updatedGoal.TargetDate;
            existingGoal.ParentGoalId = updatedGoal.ParentGoalId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id, bool deleteAll)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null || goal.UserId != userId) { return NotFound(); }

            if (!deleteAll)
            {
                await _context.Goals
                    .Where(G => G.UserId == userId)
                    .Where(G => G.ParentGoalId == id)
                    .ExecuteUpdateAsync(S => S.SetProperty(G => G.ParentGoalId, (int?)null));

                await _context.Tasks
                    .Where(T => T.UserId == userId)
                    .Where(T => T.GoalId == id)
                    .ExecuteUpdateAsync(S => S.SetProperty(T => T.GoalId, (int?)null));
            }

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskGoal(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return await _context.Tasks
                .Where(T => T.UserId == userId)
                .Where(T => T.GoalId == id)
                .ToListAsync();
        }
    }
}
