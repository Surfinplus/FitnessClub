using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;

namespace Fitnesclubplus.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrainerAvailabilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerAvailabilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Belirli bir hocanın çalışma saatlerini listele
        public async Task<IActionResult> Index(int? trainerId)
        {
            if (trainerId == null) return RedirectToAction("Index", "Trainers");

            ViewBag.TrainerId = trainerId;

            // Hocanın adını başlıkta göstermek için alalım
            var trainer = await _context.Trainers.FindAsync(trainerId);
            ViewBag.TrainerName = trainer?.FullName;

            var availabilities = _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .Where(t => t.TrainerId == trainerId)
                .OrderBy(t => t.DayOfWeek) // Gün sırasına göre dizelim
                .ThenBy(t => t.StartTime); // Saat sırasına göre

            return View(await availabilities.ToListAsync());
        }

        // 2. Yeni Saat Ekleme Sayfası
        public IActionResult Create(int trainerId)
        {
            var trainer = _context.Trainers.Find(trainerId);
            if (trainer == null) return NotFound();

            ViewBag.TrainerName = trainer.FullName;

            // Modeli hazırlayıp gönderiyoruz ki TrainerId kaybolmasın
            var model = new TrainerAvailability { TrainerId = trainerId };
            return View(model);
        }

        // 3. Kaydetme İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TrainerId,DayOfWeek,StartTime,EndTime")] TrainerAvailability trainerAvailability)
        {
            // Basit bir validasyon: Başlangıç saati Bitişten büyük olamaz
            if (trainerAvailability.StartTime >= trainerAvailability.EndTime)
            {
                ModelState.AddModelError("", "Başlangıç saati, bitiş saatinden önce olmalıdır.");
            }

            // Model validasyonu (Navigation property'leri yoksay)
            ModelState.Remove("Trainer");

            if (ModelState.IsValid)
            {
                _context.Add(trainerAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { trainerId = trainerAvailability.TrainerId });
            }

            // Hata varsa tekrar aynı sayfaya dön
            var trainer = _context.Trainers.Find(trainerAvailability.TrainerId);
            ViewBag.TrainerName = trainer?.FullName;
            return View(trainerAvailability);
        }

        // 4. Silme İşlemi
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var availability = await _context.TrainerAvailabilities.FindAsync(id);
            if (availability != null)
            {
                _context.TrainerAvailabilities.Remove(availability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { trainerId = availability.TrainerId });
            }
            return RedirectToAction("Index", "Trainers");
        }
    }
}