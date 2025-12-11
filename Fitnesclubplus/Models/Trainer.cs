using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Models
{
    public class Trainer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; }

        [Display(Name = "Uzmanlık Alanı")]
        public string Specialization { get; set; } // Örn: Kilo Verme, Kas Yapma

        // Antrenörün verebildiği hizmet (Basitlik için 1 antrenör 1 ana hizmete bağlı varsayalım, gerekirse çoka-çok yaparız)
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        // Müsaitlik veya çalışma saatleri mantığı buraya eklenebilir, şimdilik basit tutalım.
        public bool IsAvailable { get; set; } = true;
    }
}