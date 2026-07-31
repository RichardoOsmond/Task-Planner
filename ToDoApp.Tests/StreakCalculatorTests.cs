using ToDoApp.Helpers;

namespace ToDoApp.Tests
{
    public class StreakCalculatorTests
    {
        // Streak should be 0 if there are no completed dates
        [Fact]
        public void CalculateStreak_NoCompletion_ReturnsZero()
        {
            var today = new DateTime(2026, 7, 30);
            var dates = new HashSet<DateTime>();

            var result = StreakCalculator.CalculateStreak(dates, today);
            Assert.Equal(0, result);
        }

        // Streak should be 1 if only today's tasks has been completed
        [Fact]
        public void CalculateStreak_OnlyToday_ReturnsOne()
        {
            var today = new DateTime(2026, 7, 30);
            var dates = new HashSet<DateTime>
            {
                new DateTime(2026, 7, 30)
            };

            var result = StreakCalculator.CalculateStreak(dates, today);
            Assert.Equal(1, result);
        }

        // Streak should be 3 if there are 3 tasks done in 3 days in a row (At least 1 done per day in a row for 3 days)
        [Fact]
        public void CalculateStreak_ConsecutiveDays_ReturnsThree()
        {
            var today = new DateTime(2026, 7, 30);
            var dates = new HashSet<DateTime>
            {
                new DateTime(2026, 7, 30),
                new DateTime(2026, 7, 29),
                new DateTime(2026, 7, 28)
            };

            var result = StreakCalculator.CalculateStreak(dates, today);
            Assert.Equal(3, result);
        }

        // Streak should be 1 if tasks has been completed today but not yesterday
        [Fact]
        public void CalculateStreak_GapBreaks_ReturnsOne()
        {
            var today = new DateTime(2026, 7, 30);
            var dates = new HashSet<DateTime>
            {
                new DateTime(2026, 7, 30),
                new DateTime(2026, 7, 27),
                new DateTime(2026, 7, 26)
            };

            var result = StreakCalculator.CalculateStreak(dates, today);
            Assert.Equal(1, result);
        }

        // Streak should be 2 if nothing has been completed today but tasks on yesterday and the day before yesterday was done
        [Fact]
        public void CalculateStreak_Grace_ReturnsTwo()
        {
            var today = new DateTime(2026, 7, 30);
            var dates = new HashSet<DateTime>
            {
                new DateTime(2026, 7, 29),
                new DateTime(2026, 7, 28)
            };

            var result = StreakCalculator.CalculateStreak(dates, today);
            Assert.Equal(2, result);
        }

        // Streak should be 0 if today and yesterday's tasks have not been completed
        [Fact]
        public void CalculateStreak_MissedTodayYesterday_ReturnsZero()
        {
            var today = new DateTime(2026, 7, 30);
            var dates = new HashSet<DateTime>
            {
                new DateTime(2026, 7, 28)
            };

            var result = StreakCalculator.CalculateStreak(dates, today);
            Assert.Equal(0, result);
        }
    }
}