using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnesclubplus.Models
{
    public class Gym
    {
        public Gym()
        {
            // Listeleri boş olarak başlatıyoruz ki hata almayalım
            Services = new HashSet<Service>();
            Trainers = new HashSet<Trainer>();
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

        // --- RESİM İŞLEMLERİ ---
        [Display(Name = "Salon Resmi")]
        public string? ImageName { get; set; }

        [NotMapped]
        [Display(Name = "Resim Yükle")]
        public IFormFile? ImageUpload { get; set; }
        // -----------------------

        // İLİŞKİLER
        public virtual ICollection<Service> Services { get; set; }

        // YENİ: Bu salondaki personeller
        public virtual ICollection<Trainer> Trainers { get; set; }
    }
}