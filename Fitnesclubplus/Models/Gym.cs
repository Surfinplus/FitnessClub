using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Http; // IFormFile için gerekli
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // [NotMapped] için gerekli

namespace Fitnesclubplus.Models
{
    public class Gym
    {
        public Gym()
        {
            Services = new HashSet<Service>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Salon adı zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Salon Adı")]
        public string Name { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Çalışma Saatleri")]
        public string OpeningHours { get; set; }

        // --- YENİ EKLENEN KISIMLAR ---

        // 1. Veritabanına Kaydedilecek Dosya Adı
        // Örnek: "salon_123.jpg"
        [Display(Name = "Salon Resmi")]
        public string? ImageName { get; set; }

        // 2. Formdan Dosya Yüklemek İçin Yardımcı Özellik
        // [NotMapped] : Bu özellik veritabanında bir kolon oluşturmaz, sadece dosya transferi içindir.
        [NotMapped]
        [Display(Name = "Resim Yükle")]
        public IFormFile? ImageUpload { get; set; }

        // -----------------------------

        public virtual ICollection<Service> Services { get; set; }
    }
}