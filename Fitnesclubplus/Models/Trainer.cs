using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Fitnesclubplus.Models
{
    public class Trainer
    {
        public Trainer()
        {
            Services = new HashSet<Service>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; }

        [Display(Name = "Uzmanlık Alanı")]
        public string Specialization { get; set; } // Örn: Kilo Verme, Kas Yapma

        // --- YENİ: RESİM İŞLEMLERİ ---
        [Display(Name = "Personel Resmi")]
        public string? ImageName { get; set; }

        [NotMapped]
        [Display(Name = "Resim Yükle")]
        public IFormFile? ImageUpload { get; set; }
        // -----------------------------

        // --- YENİ: HANGİ SALONDA ÇALIŞIYOR? ---
        [Display(Name = "Bağlı Olduğu Salon")]
        public int GymId { get; set; }

        [ForeignKey("GymId")]
        public virtual Gym Gym { get; set; }

        // --- YENİ: VERDİĞİ HİZMETLER ---
        // Bir personel birden fazla hizmet verebilir (Örn: Hem Yüzme hem Fitness)
        public virtual ICollection<Service> Services { get; set; }
    }
}