using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ddacAPI.Data;
using ddacAPI.Models;
using ddacAPI.Util;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ddacAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();

                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                if (!await roleManager.RoleExistsAsync("Partner"))
                    await roleManager.CreateAsync(new IdentityRole { Name = "Partner" });
                if (!await roleManager.RoleExistsAsync("Customer"))
                    await roleManager.CreateAsync(new IdentityRole { Name = "Customer" });

                try
                {
                    var context = services.GetRequiredService<ddacAPIContext>();
                    services.GetRequiredService<ddacAPIContext>();
                    context.Database.Migrate();
                    SeedData.Initialize(services, userManager);

                }
                catch (Exception ex)
                {
                    var logger =
                    services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            await host.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
