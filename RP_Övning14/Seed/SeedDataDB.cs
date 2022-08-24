using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RP_Övning14.Data;
using RP_Övning14.Models;

namespace RP_Övning14.Seed
{
    public class SeedDataDB
    {
        private static ApplicationDbContext db = default;
        private static RoleManager<IdentityRole> roleManager;
        private static UserManager<ApplicationUser> userManager;

        public static async Task SeedTheData(ApplicationDbContext context, IServiceProvider services, string adminPW)
        {
            if(context == null)throw new ArgumentNullException(nameof(db));
            
            db = context;

            ArgumentNullException.ThrowIfNull(nameof(services));

            EnsureDeleted(db);

            roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var roleNames = new[] { "Member", "Admin" };
            var adminEmail = "admin@gym.se";
            //AdminPW123
            var gymClasses = GetGymClasses();
            await db.AddRangeAsync(gymClasses);

            await AddRolesAsync(roleNames);

            var admin = await AddAdminAsync(adminEmail, adminPW);

            await AddToRolesAsync(admin, roleNames);
        }

        private static async Task AddToRolesAsync(ApplicationUser admin, string[] roleNames)
        {
            foreach (var role in roleNames)
            {
                if (await userManager.IsInRoleAsync(admin, role)) continue;
                var result = await userManager.AddToRoleAsync(admin, role);
                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static async Task<ApplicationUser> AddAdminAsync(string adminEmail, string adminPW)
        {
            var found = await userManager.FindByEmailAsync(adminEmail);

            if (found != null) return null!;

            var admin = new ApplicationUser
            {
                FirstName = "Adam",
                LastName = "Aven",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPW);
            if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));

            return admin;
        }

        private static async Task AddRolesAsync(string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName)) continue;
                var role = new IdentityRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded) throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static IEnumerable<GymClass> GetGymClasses()
        {
            var faker = new Faker("sv");

            var gymClasses = new List<GymClass>();

            for (int i = 0; i < 20; i++)
            {
                var temp = new GymClass
                {
                    Name = faker.Company.CatchPhrase(),
                    Description = faker.Hacker.Verb(),
                    Duration = new TimeSpan(0, 55, 0),
                    StartTime = DateTime.Now.AddDays(faker.Random.Int(-5, 5))
                };

                gymClasses.Add(temp);
            }

            return gymClasses;
        }

        private static void EnsureDeleted(ApplicationDbContext _db)
        {

            _db.Database.EnsureDeleted();
            _db.Database.Migrate();
        }
    }
}
