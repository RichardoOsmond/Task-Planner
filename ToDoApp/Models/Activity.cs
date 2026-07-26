namespace ToDoApp.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public ActivityType ActivityType { get; set; }
        public DateTime CreatedDate { get; set; }
        public User User { get; set; } = null!;
    }
}