using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // IdentityUser için gerekli

namespace Fitnesclubplus.Models
{
    // Randevu Durumları
    public enum AppointmentStatus
    {
        Pending,    // Bekliyor
        Confirmed,  // Onaylandı
        Cancelled   // İptal Edildi
    }

    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        // --- 1. NE ZAMAN? ---
        [Required(ErrorMessage = "Tarih ve saat seçimi zorunludur.")]
        [Display(Name = "Randevu Tarihi")]
        public DateTime AppointmentDate { get; set; }

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // --- 2. DURUMU NE? ---
        [Display(Name = "Durum")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        // --- 3. KİM ALIYOR? (Müşteri) ---
        [Display(Name = "Müşteri")]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual IdentityUser? User { get; set; } // İlişkiyi buraya ekledik

        // --- 4. HANGİ HİZMET? ---
        [Required]
        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service? Service { get; set; }

        // --- 5. HANGİ EĞİTMEN? (YENİ VE KRİTİK KISIM) ---
        // Müsaitlik kontrolünü hızlı yapmak için bu şart
        [Required]
        [Display(Name = "Eğitmen")]
        public int TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        public virtual Trainer? Trainer { get; set; }
    }
}