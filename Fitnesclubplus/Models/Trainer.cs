using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Fitnesclubplus.Models
{
    public class Trainer
    {
        public Trainer()
        {
<<<<<<< HEAD
            // Hata almamak için listeleri başlatıyoruz
            Services = new HashSet<Service>();
            TrainerAvailabilities = new HashSet<TrainerAvailability>(); // <--- EKLENDİ
=======
            Services = new HashSet<Service>();
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; }

        [Display(Name = "Uzmanlık Alanı")]
        public string Specialization { get; set; }

<<<<<<< HEAD
        // --- RESİM İŞLEMLERİ ---
=======
        // --- YENİ: RESİM İŞLEMLERİ ---
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        [Display(Name = "Personel Resmi")]
        public string? ImageName { get; set; }

        [NotMapped]
        [Display(Name = "Resim Yükle")]
        public IFormFile? ImageUpload { get; set; }
<<<<<<< HEAD

        // --- HANGİ SALONDA ÇALIŞIYOR? ---
=======
        // -----------------------------

        // --- YENİ: HANGİ SALONDA ÇALIŞIYOR? ---
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        [Display(Name = "Bağlı Olduğu Salon")]
        public int GymId { get; set; }

        [ForeignKey("GymId")]
        public virtual Gym Gym { get; set; }

<<<<<<< HEAD
        // --- VERDİĞİ HİZMETLER ---
        public virtual ICollection<Service> Services { get; set; }

        // --- YENİ: ÇALIŞMA SAATLERİ (Bu eksikti, ekledik) ---
        public virtual ICollection<TrainerAvailability> TrainerAvailabilities { get; set; }
=======
        // --- YENİ: VERDİĞİ HİZMETLER ---
        // Bir personel birden fazla hizmet verebilir (Örn: Hem Yüzme hem Fitness)
        public virtual ICollection<Service> Services { get; set; }
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
    }
}