
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RP_Övning14.Data;
using RP_Övning14.Models;
using System;

namespace RP_Övning14.Seed
{
    public static class AddSeedDataExtension
    {

        public static async Task<IApplicationBuilder> AddSeedData(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var service = scope.ServiceProvider;
                var db = service.GetRequiredService<ApplicationDbContext>();
                //db.Database.EnsureDeleted();
                //db.Database.Migrate();

                var config = service.GetRequiredService<IConfiguration>();
                var adminPW = "AdminPW123";

                try
                {   
                    await SeedDataDB.SeedTheData(db, service, adminPW);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return app;
        }



    }
}
