using Fitnesclubplus.Models;
using System.ComponentModel.DataAnnotations;

namespace Fitnesclubplus.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Hizmet Adı")]
        public string ServiceName { get; set; } // Örn: Yoga, Pilates

        [Display(Name = "Süre (Dakika)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Ücret")]
        public decimal Price { get; set; }

        // İlişkiler
        public int GymId { get; set; }
        public Gym Gym { get; set; }

        public ICollection<Trainer> Trainers { get; set; }
    }
}