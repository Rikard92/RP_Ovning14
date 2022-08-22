using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RP_Övning14.Models;

namespace RP_Övning14.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<GymClass> GymClasses { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ApplicationUserGymClass> ApplicationUserGymClass { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { 
            base.OnModelCreating(modelBuilder); 
            modelBuilder.Entity<ApplicationUserGymClass>().HasKey(t => new { t.ApplicationUserId, t.GymClassId }); 
        
        }

    }
}