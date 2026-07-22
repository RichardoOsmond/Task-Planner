using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GoalsController(AppDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Goal>>> GetGoals([FromQuery] int? parentGoalId)
        {
            var query = _context.Goals.AsQueryable();
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
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null) { return NotFound(); }
            return goal;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoal(int id, Goal updatedGoal)
        {
            if (id != updatedGoal.Id) { return BadRequest(); }

            var existingGoal = await _context.Goals.FindAsync(id);
            if (existingGoal == null) { return NotFound(); }

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
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null) { return NotFound(); }

            if (!deleteAll)
            {
                await _context.Goals
                    .Where(G => G.ParentGoalId == id)
                    .ExecuteUpdateAsync(S => S.SetProperty(G => G.ParentGoalId, (int?)null));

                await _context.Tasks
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
            var query = _context.Tasks.AsQueryable();
            query = query.Where(T => T.GoalId == id);
            return await query.ToListAsync();
        }
    }
}
