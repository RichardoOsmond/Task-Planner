using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ToDoApp.Models;
using ToDoApp.Data;
using ToDoApp.Helpers;
using System.Security.Principal;

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

        [HttpGet("productivity")]
        public async Task<IActionResult> GetProductivity([FromQuery] ProductivityRange range)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var timeZoneId = await _context.Users.Where(U => U.Id == userId).Select(U => U.TimeZoneId).FirstAsync();
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var localToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).Date;
            BucketGranularity bucketGranularity;
            var count = 0;
            switch (range)
            {
                case ProductivityRange.Week:
                    bucketGranularity = BucketGranularity.Day;
                    count = 7;
                    break;
                case ProductivityRange.Month:
                    bucketGranularity = BucketGranularity.Day;
                    count = 30;
                    break;
                case ProductivityRange.SixMonths:
                    bucketGranularity = BucketGranularity.Week;
                    count = 26;
                    break;
                case ProductivityRange.Year:
                    bucketGranularity = BucketGranularity.Month;
                    count = 12;
                    break;
                default:
                    bucketGranularity = BucketGranularity.Day;
                    count = 30;
                    break;
            }

            DateTime BucketStart(DateTime d) => bucketGranularity switch
            {
                BucketGranularity.Day => d,
                BucketGranularity.Week => d.AddDays(-(((int)d.DayOfWeek + 6) % 7)),
                BucketGranularity.Month => new DateTime(d.Year, d.Month, 1),
                _ => d
            };

            DateTime StepBack(DateTime b, int n) => bucketGranularity switch
            {
                BucketGranularity.Day => b.AddDays(-n),
                BucketGranularity.Week => b.AddDays(-7 * n),
                BucketGranularity.Month => b.AddMonths(-n),
                _ => b.AddDays(-n)
            };

            var currentBucket = BucketStart(localToday);
            var windowStartLocal = StepBack(currentBucket, count - 1);
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(windowStartLocal, timeZone);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(localToday.AddDays(1), timeZone);
            var completedTasks = await _context.Tasks.Where(T => T.UserId == userId && T.CompletedDate != null && 
            T.CompletedDate.Value >= startUtc && T.CompletedDate.Value < endUtc).Select(T => T.CompletedDate!.Value).ToListAsync();
            var taskCount = completedTasks.GroupBy(T => BucketStart(TimeZoneInfo.ConvertTimeFromUtc(T, timeZone).Date))
                .ToDictionary(g => g.Key, g => g.Count());
            var result = new List<object>();
            for (int i = count - 1; i >= 0; i--)
            {
                var bucket = StepBack(currentBucket, i);
                result.Add(new
                {
                    date = bucket.ToString("yyyy-MM-dd"),
                    count = taskCount.TryGetValue(bucket, out var c) ? c : 0
                });
            }
            return Ok(result);
        }

        [HttpGet("activity")]
        public async Task<IActionResult> GetActivity([FromQuery] int days = 365)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var timeZoneId = await _context.Users.Where(U => U.Id == userId).Select(U => U.TimeZoneId).FirstAsync();
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var localToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).Date;
            var windowStartLocal = localToday.AddDays(-(days - 1));
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(windowStartLocal, timeZone);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(localToday.AddDays(1), timeZone);
            var activities = await _context.Activities.Where(A => A.UserId == userId &&
            A.CreatedDate >= startUtc && A.CreatedDate < endUtc).Select(A => A.CreatedDate).ToListAsync();
            var activityCount = activities.GroupBy(A => TimeZoneInfo.ConvertTimeFromUtc(A, timeZone).Date)
                .ToDictionary(g => g.Key, g => g.Count());
            var result = new List<object>();
            for (int i = days - 1; i >= 0; i--)
            {
                var day = localToday.AddDays(-i);
                result.Add(new
                {
                    date = day.ToString("yyyy-MM-dd"),
                    count = activityCount.TryGetValue(day, out var c) ? c : 0
                });
            }
            return Ok(result);
        }
    }
}