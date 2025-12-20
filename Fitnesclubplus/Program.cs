using Fitnesclubplus;
using Fitnesclubplus.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services; // EmailSender arayüzü için

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// 1. VERİTABANI BAĞLANTISI
// =========================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// =========================================================
// 2. IDENTITY (ÜYELİK) AYARLARI
// =========================================================
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Giriş Ayarları
    options.SignIn.RequireConfirmedAccount = false; // Test için mail onayı kapalı

    // Şifre Kuralları (Senin belirlediğin basit kurallar)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Giriş yapılmamışsa yönlendirilecek sayfalar
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// =========================================================
// 3. MVC, RAZOR PAGES VE MAİL SERVİSİ
// =========================================================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// EmailSender hatasını önlemek için dummy servis
builder.Services.AddSingleton<IEmailSender, EmailSender>();

var app = builder.Build();

// =========================================================
// 4. HTTP REQUEST PIPELINE (UYGULAMA AKIŞI)
// =========================================================

// Hata Yönetimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Önce Kimlik Doğrulama (Authentication), Sonra Yetkilendirme (Authorization)
app.UseAuthentication();
app.UseAuthorization();

// Rotalar
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Identity sayfaları için gerekli

// =========================================================
// 5. OTOMATİK ROL VE YETKİLENDİRME BLOĞU
// =========================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // RoleSeeder sınıfındaki SeedRolesAsync metodunu çağırıyoruz.
        // Bu metot:
        // 1. "Admin" rolü yoksa oluşturur.
        // 2. "g221210090@sakarya.edu.tr" kullanıcısını bulursa ona "Admin" yetkisi verir.
        await RoleSeeder.SeedRolesAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine("---------------------------------------------");
        Console.WriteLine("DİKKAT - Rol Atama Hatası: " + ex.Message);
        Console.WriteLine("---------------------------------------------");
    }
}

app.Run();