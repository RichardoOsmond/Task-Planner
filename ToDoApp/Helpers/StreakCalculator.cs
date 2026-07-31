namespace ToDoApp.Helpers
{
    public static class StreakCalculator
    {
        public static int CalculateStreak(HashSet<DateTime> completedDates, DateTime today)
        {
            var streak = 0;
            var pointer = today;
            if (!completedDates.Contains(pointer))
            {
                pointer = pointer.AddDays(-1);
            }
            while (completedDates.Contains(pointer))
            {
                streak++;
                pointer = pointer.AddDays(-1);
            }
            return streak;
        }
    }
}
