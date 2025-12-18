using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnesclubplus.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        // Hata listesindeki 'ServiceName' beklentisi için:
        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [Display(Name = "Hizmet Adı")]
        public string ServiceName { get; set; }

        // Hata listesindeki 'Price' beklentisi için:
        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Display(Name = "Ücret (TL)")]
        public decimal Price { get; set; }

        // Hata listesindeki 'DurationMinutes' beklentisi için:
        [Required(ErrorMessage = "Süre zorunludur.")]
        [Display(Name = "Süre (Dakika)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        // --- Gym İlişkisi ---
        [Display(Name = "Spor Salonu")]
        public int GymId { get; set; }

        [ForeignKey("GymId")]
        public virtual Gym Gym { get; set; }
    }
}