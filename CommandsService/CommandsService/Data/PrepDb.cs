using System;
using System.Collections.Generic;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data 
{ 

    public static class PrepDb
    {
        public static void PrepPopulation(WebApplication app)
        {
            using (var serviceScope = app.Services.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

               // var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                if (grpcClient == null)
                {
                    throw new Exception("gRPC client is not registered or resolved correctly.");
                }

               // var platforms = grpcClient.ReturnAllPlatforms();

                var platforms = grpcClient.ReturnAllPlatforms();
                if (platforms == null)
                {
                    Console.WriteLine("No platforms were returned from the gRPC service.");
                    return; // Optionally handle the case where no platforms are returned.
                }

                SeedData(serviceScope.ServiceProvider.GetRequiredService<ICommandRepo>(), platforms);
            }
        }

        private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("Seeding new platforms...");

            foreach (var plat in platforms)
            {
                if (!repo.ExternalPlatformExists(plat.ExternalID))
                {
                    repo.CreatePlatform(plat);
                }
                repo.SaveChanges();
            }
        }


    }
}