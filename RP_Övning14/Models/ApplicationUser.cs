using Microsoft.AspNetCore.Identity;

namespace RP_Övning14.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }

    }
}
