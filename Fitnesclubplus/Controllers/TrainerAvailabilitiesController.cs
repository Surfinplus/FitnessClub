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

<<<<<<< HEAD
        // 1. LİSTELEME
=======
        // 1. Belirli bir hocanın çalışma saatlerini listele
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        public async Task<IActionResult> Index(int? trainerId)
        {
            if (trainerId == null) return RedirectToAction("Index", "Trainers");

            ViewBag.TrainerId = trainerId;
<<<<<<< HEAD
            var trainer = await _context.Trainers.FindAsync(trainerId);
            ViewBag.TrainerName = trainer?.FullName;

            // Hem genel kayıtları (Date == null) hem de gelecek tarihli kayıtları getir
            var availabilities = _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .Where(t => t.TrainerId == trainerId && (t.IsGeneral == true || t.Date >= DateTime.Today))
                .OrderByDescending(t => t.IsGeneral) // Önce "Her Gün" olanlar görünsün
                .ThenBy(t => t.Date)
                .ThenBy(t => t.StartTime);
=======

            // Hocanın adını başlıkta göstermek için alalım
            var trainer = await _context.Trainers.FindAsync(trainerId);
            ViewBag.TrainerName = trainer?.FullName;

            var availabilities = _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .Where(t => t.TrainerId == trainerId)
                .OrderBy(t => t.DayOfWeek) // Gün sırasına göre dizelim
                .ThenBy(t => t.StartTime); // Saat sırasına göre
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138

            return View(await availabilities.ToListAsync());
        }

<<<<<<< HEAD
        // 2. EKLEME SAYFASI
=======
        // 2. Yeni Saat Ekleme Sayfası
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        public IActionResult Create(int trainerId)
        {
            var trainer = _context.Trainers.Find(trainerId);
            if (trainer == null) return NotFound();

            ViewBag.TrainerName = trainer.FullName;

<<<<<<< HEAD
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

=======
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
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
            ModelState.Remove("Trainer");

            if (ModelState.IsValid)
            {
                _context.Add(trainerAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { trainerId = trainerAvailability.TrainerId });
            }

<<<<<<< HEAD
=======
            // Hata varsa tekrar aynı sayfaya dön
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
            var trainer = _context.Trainers.Find(trainerAvailability.TrainerId);
            ViewBag.TrainerName = trainer?.FullName;
            return View(trainerAvailability);
        }

<<<<<<< HEAD
        // 4. SİLME İŞLEMİ
=======
        // 4. Silme İşlemi
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
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