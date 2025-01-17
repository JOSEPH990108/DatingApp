﻿using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {

        public static async Task ClearConnections(ApplicationDbContext db)
        {
            db.Connections.RemoveRange(db.Connections);
            await db.SaveChangesAsync();
        }

        public static async Task SeedUsers(UserManager<ApplicationUser> userManager, RoleManager<AppRole> roleManager)
        {
            if(await userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/DatabaseSeeds/ApplicationUserSeedData.json");

            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

            var users = JsonSerializer.Deserialize<List<ApplicationUser>>(userData, options);

            var roles = new List<AppRole>
            {
                new AppRole { Name = "Admin" },
                new AppRole { Name = "Member" },
                new AppRole { Name = "Moderator" }
            };

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach(var user in users)
            {
                user.UserName = user.UserName.ToLower();
                user.Created = DateTime.SpecifyKind(user.Created, DateTimeKind.Utc);
                user.LastActive = DateTime.SpecifyKind(user.LastActive, DateTimeKind.Utc);
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new ApplicationUser
            {
                UserName = "admin"              
            };

            await userManager.CreateAsync(admin, "Admin123");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
        }
    }
}


