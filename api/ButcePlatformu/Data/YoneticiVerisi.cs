using Microsoft.AspNetCore.Identity;

namespace ButcePlatformu.Data;

public static class YoneticiVerisi
{
    public static async Task OlusturAsync(IServiceProvider servisler)
    {
        var rolYoneticisi = servisler.GetRequiredService<RoleManager<IdentityRole>>();
        var kullaniciYoneticisi = servisler.GetRequiredService<UserManager<IdentityUser>>();

        string[] roller = { "Admin", "Kullanici" };
        foreach (var rol in roller)
        {
            if (!await rolYoneticisi.RoleExistsAsync(rol))
                await rolYoneticisi.CreateAsync(new IdentityRole(rol));
        }

        const string adminEposta = "admin@butce.local";
        if (await kullaniciYoneticisi.FindByEmailAsync(adminEposta) == null)
        {
            var admin = new IdentityUser
            {
                UserName = adminEposta,
                Email = adminEposta,
                EmailConfirmed = true
            };
            await kullaniciYoneticisi.CreateAsync(admin, "Yonetici2026!");
            await kullaniciYoneticisi.AddToRoleAsync(admin, "Admin");
        }
    }
}