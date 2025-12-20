using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnesclubplus.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        // ESKİ: public string ServiceName { get; set; } -> KALDIRILDI

        // YENİ: HİZMET HAVUZUNDAN SEÇİM
        [Display(Name = "Hizmet Kategorisi")]
        public int ServiceCategoryId { get; set; }

        [ForeignKey("ServiceCategoryId")]
        [Display(Name = "Hizmet Adı")]
        public virtual ServiceCategory ServiceCategory { get; set; }

        // --- ORTAK ALANLAR ---
        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Display(Name = "Ücret (TL)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Süre zorunludur.")]
        [Display(Name = "Süre (Dakika)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        // --- İLİŞKİLER ---

        // 1. Hangi Salon? (Veri tutarlılığı için tutuyoruz)
        [Display(Name = "Spor Salonu")]
        public int GymId { get; set; }
        [ForeignKey("GymId")]
        public virtual Gym Gym { get; set; }

        // 2. Hangi Personel Veriyor? (YENİ)
        // Eğer hizmeti bir hoca veriyorsa burası dolar. Hoca yoksa (sadece salon kullanımı) boş olabilir.
        [Display(Name = "Eğitmen / Personel")]
        public int? TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        public virtual Trainer? Trainer { get; set; }
    }
}