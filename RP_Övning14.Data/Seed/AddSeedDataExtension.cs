
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using RP_Övning14.Data;
using RP_Övning14.Seed;
using System;

namespace RP_Övning14.Data.Seed;
    public static class AddSeedDataExtension
    {

        public static async Task<IApplicationBuilder> AddSeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
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


