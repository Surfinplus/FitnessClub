using Microsoft.AspNetCore.Identity;

namespace Fitnesclubplus.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            // Rol Yöneticisini çağırıyoruz
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Oluşturmak istediğimiz rollerin listesi
            string[] roleNames = { "Admin", "Member" };

            foreach (var roleName in roleNames)
            {
                // Rol zaten var mı diye kontrol et
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    // Yoksa oluştur!
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}