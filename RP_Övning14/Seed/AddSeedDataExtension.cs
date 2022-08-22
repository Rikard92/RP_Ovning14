
using Microsoft.EntityFrameworkCore;
using RP_Övning14.Data;
using RP_Övning14.Models;

namespace RP_Övning14.Seed
{
    public static class AddSeedDataExtension
    {

        public static async Task AddSeedData(this WebApplication appl)
        {
            using (var scope = appl.Services.CreateScope())
            {
                try
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await db.AddSeedData();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }

        public static async Task AddSeedData(this ApplicationDbContext db)
        {

            try
            {
                await SeedDataDB.Run(db);
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
