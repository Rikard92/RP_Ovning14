using Microsoft.AspNetCore.Identity;

namespace RP_Övning14.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public DateTime TimeOfRegistration { get; set; }
        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }

    }
}
