using Fitnesclubplus;
using Fitnesclubplus.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Identity (Üyelik ve Rol) Ayarları
// DİKKAT: Burası tek ve yetkili Identity ayarıdır. Başka bir AddDefaultIdentity satırı olmamalı.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Giriş Ayarları
    options.SignIn.RequireConfirmedAccount = false; // Mail onayı zorunluluğunu kaldırdık (Test için)

    // Şifre Kuralları (Test için basitleştirdik, istersen değiştirebilirsin)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Giriş yapılmamışsa yönlendirilecek sayfalar (Login yolunu gösteriyoruz)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// 3. MVC ve Razor Pages Servisleri
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Scaffolding ile eklenen Identity sayfaları için ŞART
// Mail servisi hatasını çözmek için eklediğimiz satır:
builder.Services.AddSingleton<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();
var app = builder.Build();

// Hata Yönetimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Önce Kimlik Doğrulama, Sonra Yetkilendirme (Sıralama Önemli)
app.UseAuthentication();
app.UseAuthorization();

// Rotalar
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Identity sayfalarının (Login/Register) çalışması için

// --- ROLLERİ OTOMATİK OLUŞTURMA VE ADMİN ATAMA BLOĞU ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // RoleSeeder sınıfını çalıştırıyoruz
        await RoleSeeder.SeedRolesAsync(services);
    }
    catch (Exception ex)
    {
        // Hata olursa konsola yaz ama siteyi durdurma
        Console.WriteLine("---------------------------------------------");
        Console.WriteLine("DİKKAT - Rol/Admin Oluşturma Hatası: " + ex.Message);
        Console.WriteLine("---------------------------------------------");
    }
}
// --------------------------------------------------------

app.Run();