using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnesclubplus.Models
{
    // Randevu Durumları için Sabit Liste (Enum)
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
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Kayıt anındaki zamanı otomatik alır

        // --- 2. DURUMU NE? ---
        [Display(Name = "Durum")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending; // Varsayılan: Bekliyor

        // --- 3. KİM ALIYOR? (Kullanıcı) ---
        // Identity sistemindeki User ID'si genellikle 'string' (Guid) olarak tutulur.
        [Display(Name = "Müşteri")]
        public string? UserId { get; set; }

        // --- 4. HANGİ HİZMET? ---
        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service? Service { get; set; }
    }
}