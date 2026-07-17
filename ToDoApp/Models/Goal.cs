namespace ToDoApp.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ParentGoalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? TargetDate { get; set; }
        public Goal? ParentGoal { get; set; }
        public ICollection<Goal> SubGoals { get; set; } = new List<Goal>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
