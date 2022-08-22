using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RP_Övning14.Data;
using RP_Övning14.Models;

namespace RP_Övning14.Seed
{
    public class SeedDataDB
    {
        public static async Task Run(ApplicationDbContext db)
        {

            try
            {
                //EnsureDeleted(db);

                //Seed Rols
                db.Roles.Add(new IdentityRole
                {
                    Name = "Admin"
                    
                });
                db.Roles.Add(new IdentityRole
                {
                    Name = "Student"

                });

                //Seed Admin
                ApplicationUser Admin = new ApplicationUser
                {
                    Id = "0123",
                    UserName = "Admin",
                    Email = "admin@Gymbokning.se",
                    PasswordHash = ""
                                        
                };
                db.AddRange(Admin);

                db.UserRoles.Add(new IdentityUserRole<string>
                {

                });



                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {

                throw;
            }
        }



        private static void EnsureDeleted(ApplicationDbContext _db)
        {

            _db.Database.EnsureDeleted();
            _db.Database.Migrate();
        }
    }
}
