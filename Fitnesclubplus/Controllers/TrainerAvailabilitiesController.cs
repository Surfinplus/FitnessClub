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

        // 1. LİSTELEME
        public async Task<IActionResult> Index(int? trainerId)
        {
            if (trainerId == null) return RedirectToAction("Index", "Trainers");

            ViewBag.TrainerId = trainerId;
            var trainer = await _context.Trainers.FindAsync(trainerId);
            ViewBag.TrainerName = trainer?.FullName;

            // Hem genel kayıtları (Date == null) hem de gelecek tarihli kayıtları getir
            var availabilities = _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .Where(t => t.TrainerId == trainerId && (t.IsGeneral == true || t.Date >= DateTime.Today))
                .OrderByDescending(t => t.IsGeneral) // Önce "Her Gün" olanlar görünsün
                .ThenBy(t => t.Date)
                .ThenBy(t => t.StartTime);

            return View(await availabilities.ToListAsync());
        }

        // 2. EKLEME SAYFASI
        public IActionResult Create(int trainerId)
        {
            var trainer = _context.Trainers.Find(trainerId);
            if (trainer == null) return NotFound();

            ViewBag.TrainerName = trainer.FullName;

            var model = new TrainerAvailability
            {
                TrainerId = trainerId,
                Date = DateTime.Today.AddDays(1), // Varsayılan yarın
                IsGeneral = false
            };
            return View(model);
        }

        // 3. KAYDETME İŞLEMİ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TrainerId,IsGeneral,Date,StartTime,EndTime")] TrainerAvailability trainerAvailability)
        {
            // Validasyon 1: Eğer 'Genel' seçilmediyse Tarih zorunludur
            if (!trainerAvailability.IsGeneral && trainerAvailability.Date == null)
            {
                ModelState.AddModelError("Date", "Eğer 'Her Gün' seçeneğini işaretlemediyseniz bir tarih seçmelisiniz.");
            }

            // Validasyon 2: Tarih seçildiyse geçmiş olamaz
            if (!trainerAvailability.IsGeneral && trainerAvailability.Date < DateTime.Today)
            {
                ModelState.AddModelError("Date", "Geçmiş bir tarihe uygunluk ekleyemezsiniz.");
            }

            // Validasyon 3: Saat kontrolü
            if (trainerAvailability.StartTime >= trainerAvailability.EndTime)
            {
                ModelState.AddModelError("EndTime", "Bitiş saati, başlangıç saatinden sonra olmalıdır.");
            }

            // Eğer Genel seçildiyse Tarihi NULL yapalım ki veritabanı karışmasın
            if (trainerAvailability.IsGeneral)
            {
                trainerAvailability.Date = null;
            }

            ModelState.Remove("Trainer");

            if (ModelState.IsValid)
            {
                _context.Add(trainerAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { trainerId = trainerAvailability.TrainerId });
            }

            var trainer = _context.Trainers.Find(trainerAvailability.TrainerId);
            ViewBag.TrainerName = trainer?.FullName;
            return View(trainerAvailability);
        }

        // 4. SİLME İŞLEMİ
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