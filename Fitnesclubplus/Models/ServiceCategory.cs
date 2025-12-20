using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Fitnesclubplus.Models
{
    public class ServiceCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [Display(Name = "Hizmet Kategorisi")]
        public string Name { get; set; } // Örn: Yüzme, Pilates, Boks

        // Bu kategoriden türetilmiş hizmetlerin listesi (Opsiyonel navigation property)
        public virtual ICollection<Service> Services { get; set; }
    }
}