using Microsoft.AspNetCore.Identity;

namespace ToDoApp.Models
{
    public class User : IdentityUser<int>
    {
        public DateTime CreatedDate { get; set;  }
        public DateOnly? DateOfBirth {  get; set; }
        public string TimeZoneId { get; set; } = "Asia/Kuala_Lumpur";
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}