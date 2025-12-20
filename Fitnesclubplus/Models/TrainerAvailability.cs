using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnesclubplus.Models
{
    public class TrainerAvailability
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        public virtual Trainer Trainer { get; set; }

        // YENİ: Bu kutucuk işaretlenirse tarih önemsiz olacak
        [Display(Name = "Her Gün Geçerli")]
        public bool IsGeneral { get; set; }

        // YENİ: Tarih artık boş olabilir (soru işareti ekledik)
        [Display(Name = "Tarih")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required]
        [Display(Name = "Başlangıç Saati")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "Bitiş Saati")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
    }
}