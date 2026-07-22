using Microsoft.AspNetCore.Identity;

namespace ToDoApp.Models
{
    public class User : IdentityUser<int>
    {
        public DateTime CreatedDate { get; set;  }
        public DateOnly? DateOfBirth {  get; set; }
    }
}
