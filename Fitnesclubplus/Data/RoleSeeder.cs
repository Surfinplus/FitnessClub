using Microsoft.AspNetCore.Identity;

namespace Fitnesclubplus.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Admin", "Member" };

            // 1. Rolleri Oluştur (Eğer yoksa)
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Özel Admin Kullanıcısını Bul ve Yetki Ver
            // BURAYI GÜNCELLEDİK:
            var adminEmail = "g221210090@sakarya.edu.tr";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser != null)
            {
                // Eğer kullanıcı bulunduysa ve Admin değilse, Admin yap.
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}