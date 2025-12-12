using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fitnesclubplus.Data
{
    // IdentityDbContext'ten miras alıyoruz. Bu sayede Users, Roles gibi tablolar otomatik gelecek.
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Modelleri veritabanı tablosu olarak tanıtıyoruz
        public DbSet<Gym> Gyms { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Identity ayarlarının bozulmaması için base.OnModelCreating çağrılmalıdır.
        }
    }
}