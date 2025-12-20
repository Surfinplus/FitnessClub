using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnesclubplus.Models
{
    public class TrainerAvailability
    {
        [Key]
        public int Id { get; set; }

<<<<<<< HEAD
=======
        // Hangi Hoca?
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        [Required]
        public int TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        public virtual Trainer Trainer { get; set; }

<<<<<<< HEAD
        // YENİ: Bu kutucuk işaretlenirse tarih önemsiz olacak
        [Display(Name = "Her Gün Geçerli")]
        public bool IsGeneral { get; set; }

        // YENİ: Tarih artık boş olabilir (soru işareti ekledik)
        [Display(Name = "Tarih")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

=======
        // Hangi Gün? (Pazartesi, Salı vs.)
        [Required]
        [Display(Name = "Gün")]
        public DayOfWeek DayOfWeek { get; set; }

        // Saat Kaçta Başlıyor?
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        [Required]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

<<<<<<< HEAD
=======
        // Saat Kaçta Bitiyor?
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        [Required]
        [Display(Name = "Bitiş Saati")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
    }
}