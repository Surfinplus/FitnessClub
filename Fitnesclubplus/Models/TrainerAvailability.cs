using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnesclubplus.Models
{
    public class TrainerAvailability
    {
        [Key]
        public int Id { get; set; }

        // Hangi Hoca?
        [Required]
        public int TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        public virtual Trainer Trainer { get; set; }

        // Hangi Gün? (Pazartesi, Salı vs.)
        [Required]
        [Display(Name = "Gün")]
        public DayOfWeek DayOfWeek { get; set; }

        // Saat Kaçta Başlıyor?
        [Required]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        // Saat Kaçta Bitiyor?
        [Required]
        [Display(Name = "Bitiş Saati")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
    }
}