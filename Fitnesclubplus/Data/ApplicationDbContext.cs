using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fitnesclubplus.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gym> Gyms { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }

        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        // YENİ EKLENEN TABLO
        public DbSet<ServiceCategory> ServiceCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
<<<<<<< HEAD

            // --- HATA ÇÖZEN KOD BLOĞU BAŞLANGICI ---
            // Bu ayar, "Multiple cascade paths" hatasını engeller.
            // Eğitmen silindiğinde Randevuları otomatik silmeye çalışmaz.
            builder.Entity<Appointment>()
                .HasOne(a => a.Trainer)
                .WithMany()
                .HasForeignKey(a => a.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
            // --- HATA ÇÖZEN KOD BLOĞU BİTİŞİ ---
=======
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        }
    }
}