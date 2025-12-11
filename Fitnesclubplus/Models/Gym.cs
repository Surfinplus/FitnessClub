using Fitnesclubplus.Models;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Models
{
    public class Gym
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Salon adı zorunludur.")]
        [Display(Name = "Salon Adı")]
        public string Name { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Çalışma Saatleri")]
        public string OpeningHours { get; set; } // Örn: "09:00 - 22:00"

        // Bir salonun birden fazla hizmeti olabilir
        public ICollection<Service> Services { get; set; }
    }
}