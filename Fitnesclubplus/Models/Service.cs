using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnesclubplus.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        // Eski sayfaların hata vermemesi için 'Name' yerine 'ServiceName' kullanıyoruz
        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [Display(Name = "Hizmet Adı")]
        public string ServiceName { get; set; }

        // Hata listesindeki 'Price' beklentisi için eklendi
        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Display(Name = "Ücret (TL)")]
        public decimal Price { get; set; }

        // Hata listesindeki 'DurationMinutes' beklentisi için eklendi
        [Required(ErrorMessage = "Süre zorunludur.")]
        [Display(Name = "Süre (Dakika)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        // --- İLİŞKİ AYARLARI ---
        [Display(Name = "Spor Salonu")]
        public int GymId { get; set; }

        [ForeignKey("GymId")]
        public virtual Gym Gym { get; set; }
    }
}