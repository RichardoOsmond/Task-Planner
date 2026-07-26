using Microsoft.AspNetCore.Identity;

namespace ToDoApp.Models
{
    public class User : IdentityUser<int>
    {
        public DateTime CreatedDate { get; set;  }
        public DateOnly? DateOfBirth {  get; set; }
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}