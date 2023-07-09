using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(WebApplication app)
        {
            using( var serviceScope = app.Services.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetRequiredService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext context)
        {
          
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
          
            
            if(!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...");

                context.Platforms.AddRange(
                    new Platform() {Name="Dot Net", Publisher="Microsoft", Cost="Free"},
                    new Platform() {Name="SQL Server Express", Publisher="Microsoft",  Cost="Free"},
                    new Platform() {Name="Kubernetes", Publisher="Cloud Native Computing Foundation",  Cost="Free"}
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}