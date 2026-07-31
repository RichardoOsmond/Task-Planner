using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ToDoApp.Models;
using ToDoApp.Data;
using ToDoApp.Helpers;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AnalyticsController(AppDbContext context) {  _context = context; }
        
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var timeZoneId = await _context.Users.Where(U => U.Id == userId).Select(U => U.TimeZoneId).FirstAsync();
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            
            // Retrieve the local time today (midnight) to convert into UTC and add 24 hours to the UTC time to simulate 1 day in local time using UTC
            var localToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).Date;
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(localToday, timeZone);
            var endUtc = startUtc.AddDays(1);
            var todayCount = await _context.Tasks.CountAsync(T => T.UserId == userId && T.CompletedDate != null && T.CompletedDate.Value >= startUtc && T.CompletedDate.Value < endUtc);
            var totalCompletedTasks = await _context.Tasks.CountAsync(T => T.UserId == userId && T.CompletedDate != null);
            var totalTasks = await _context.Tasks.CountAsync(T => T.UserId == userId);
            var pendingTasks = totalTasks - totalCompletedTasks;
            var completionRate = totalTasks == 0 ? 0 : (int)Math.Round((double)totalCompletedTasks / totalTasks * 100);

            // Active Goals are Parent Goals that are still unfinished
            var activeGoals = await _context.Goals.CountAsync(G => G.UserId == userId && G.ParentGoalId == null && G.Tasks.Any(T => T.CompletedDate == null));
            // Placeholder for Daily Streak. Streak means the amount of days in a row where the user finished their daily goals/tasks
            var rawCompletedDates = await _context.Tasks.Where(T => T.UserId == userId && T.CompletedDate != null).Select(T => T.CompletedDate).ToListAsync();
            var completedDates = rawCompletedDates.Select(D => TimeZoneInfo.ConvertTimeFromUtc(D!.Value, timeZone).Date).ToHashSet();
            var streak = StreakCalculator.CalculateStreak(completedDates, localToday);
            return Ok(new {todayCount, totalCompletedTasks, totalTasks, pendingTasks, completionRate, activeGoals, streak});
        }
    }
}