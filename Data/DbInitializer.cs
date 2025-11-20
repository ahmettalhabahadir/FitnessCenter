using FitnessCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Apply migrations - check if database exists first
                try
                {
                    if (await context.Database.CanConnectAsync())
                    {
                        // Check if migrations are pending
                        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                        if (pendingMigrations.Any())
                        {
                            await context.Database.MigrateAsync();
                        }
                    }
                    else
                    {
                        // Database doesn't exist, create it with migrations
                        await context.Database.MigrateAsync();
                    }
                }
                catch (Exception ex)
                {
                    // Log migration errors but don't stop application
                    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DbInitializer));
                    logger.LogWarning(ex, "Migration failed, but continuing with seeding. This is normal if database already exists.");
                }

                // Seed roles
                await SeedRolesAsync(roleManager);

                // Seed default gym for admin
                var defaultGym = await SeedDefaultGymAsync(context);

                // Seed admin user
                await SeedAdminUserAsync(userManager, context, defaultGym);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DbInitializer));
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Member" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task<Gym> SeedDefaultGymAsync(ApplicationDbContext context)
        {
            // Check if any gym exists
            var existingGym = await context.Gyms.FirstOrDefaultAsync();
            
            if (existingGym == null)
            {
                // Create default gym for admin
                existingGym = new Gym
                {
                    Name = "Ana Spor Salonu",
                    Address = "Sakarya Üniversitesi Kampüsü",
                    Phone = "0264 295 0000",
                    Email = "sporsalonu@sakarya.edu.tr",
                    OpeningTime = "06:00",
                    ClosingTime = "22:00",
                    WorkingHours = "Pazartesi - Pazar: 06:00 - 22:00",
                    Description = "Ana spor salonu"
                };
                
                context.Gyms.Add(existingGym);
                await context.SaveChangesAsync();
            }
            
            return existingGym;
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context, Gym defaultGym)
        {
            var adminEmail = "B231210038@sakarya.edu.tr";
            
            // Check if admin already exists (try both cases)
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = await userManager.FindByEmailAsync("b231210038@sakarya.edu.tr");
            }

            // If not found, create new admin
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    GymId = defaultGym.Id // Admin'e spor salonu ata
                };

                var result = await userManager.CreateAsync(adminUser, "sau");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Log errors if creation fails
                    var logger = userManager.GetType().Assembly.GetName().Name;
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating admin: {error.Description}");
                    }
                }
            }
            else
            {
                // Ensure existing admin has Admin role
                var isInRole = await userManager.IsInRoleAsync(adminUser, "Admin");
                if (!isInRole)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                
                // Ensure admin has a gym assigned
                if (adminUser.GymId == null)
                {
                    adminUser.GymId = defaultGym.Id;
                    await userManager.UpdateAsync(adminUser);
                }
                
                // Reset password to ensure it's "sau"
                var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                await userManager.ResetPasswordAsync(adminUser, token, "sau");
            }
        }
    }
}

