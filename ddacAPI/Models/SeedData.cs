using ddacAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ddacAPI.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {

            using (var context = new ddacAPIContext(
            serviceProvider.GetRequiredService<DbContextOptions<ddacAPIContext>>
            ()))
            {
                var user = new ApplicationUser
                {
                    UserName = "Admin1",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    NormalizedUserName = "ADMIN1",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    Admin = new Admin()
                    {

                    }
                };


                if (!context.Users.Any(u => u.UserName == user.UserName))
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(user, "!QAZxsw2");
                    user.PasswordHash = hashed;

                    var userStore = new UserStore<ApplicationUser>(context);
                    var result = userStore.CreateAsync(user).Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }

                // Look for any movies.
                if (context.Hotel.Any())
                {
                    return ; // DB has been seeded
                }

                context.SaveChangesAsync();
            }
        }

        public static async Task<IdentityResult> AssignRole(IServiceProvider services, ApplicationUser user, string role)
        {
            UserManager<ApplicationUser> _userManager = services.GetService<UserManager<ApplicationUser>>();
            var result = await _userManager.AddToRoleAsync(user, role);

            return result;
        }
    }
}
