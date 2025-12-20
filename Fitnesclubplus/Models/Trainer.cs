using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Fitnesclubplus.Models
{
    public class Trainer
    {
        public Trainer()
        {
            // Hata almamak için listeleri başlatıyoruz
            Services = new HashSet<Service>();
            TrainerAvailabilities = new HashSet<TrainerAvailability>(); // <--- EKLENDİ
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; }

        [Display(Name = "Uzmanlık Alanı")]
        public string Specialization { get; set; }

        // --- RESİM İŞLEMLERİ ---
        [Display(Name = "Personel Resmi")]
        public string? ImageName { get; set; }

        [NotMapped]
        [Display(Name = "Resim Yükle")]
        public IFormFile? ImageUpload { get; set; }

        // --- HANGİ SALONDA ÇALIŞIYOR? ---
        [Display(Name = "Bağlı Olduğu Salon")]
        public int GymId { get; set; }

        [ForeignKey("GymId")]
        public virtual Gym Gym { get; set; }

        // --- VERDİĞİ HİZMETLER ---
        public virtual ICollection<Service> Services { get; set; }

        // --- YENİ: ÇALIŞMA SAATLERİ (Bu eksikti, ekledik) ---
        public virtual ICollection<TrainerAvailability> TrainerAvailabilities { get; set; }
    }
}