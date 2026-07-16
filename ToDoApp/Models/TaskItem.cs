namespace ToDoApp.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public int GoalId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Duration { get; set; } // In minutes
        public string Description { get; set; } = string.Empty; // Empty if a description is never set
        public DateTime CreatedDate { get; set;  }
        public DateTime? CompletedDate { get; set; } // Nullable if the task have not been completed
        public DateTime? DueDate { get; set; } // Nullable if there's no set due date
    }
}
