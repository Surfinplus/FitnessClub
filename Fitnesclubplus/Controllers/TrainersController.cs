using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;

namespace Fitnesclubplus.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;
<<<<<<< HEAD
        // Resim kaydetmek için ortam bilgisi (wwwroot yolunu bulmak için)
=======
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        private readonly IWebHostEnvironment _hostEnvironment;

        public TrainersController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

<<<<<<< HEAD
        // --- 1. LİSTELEME ---
=======
        // GET: Index
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        public async Task<IActionResult> Index()
        {
            var trainers = _context.Trainers.Include(t => t.Gym);
            return View(await trainers.ToListAsync());
        }

<<<<<<< HEAD
        // --- 2. DETAY ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.Services)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // --- 3. EKLEME SAYFASI ---
=======
        // GET: Create
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        public IActionResult Create()
        {
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name");
            return View();
        }

<<<<<<< HEAD
        // --- 4. EKLEME İŞLEMİ (RESİM YÜKLEME KODU EKLENDİ) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trainer trainer)
        {
            // Validasyon Hatası Çözümü: 
            // Model, Gym nesnesinin tamamını bekler ama biz sadece ID gönderiyoruz.
            // Bu yüzden Gym, Services ve Availability alanlarını validasyondan çıkarıyoruz.
            ModelState.Remove("Gym");
            ModelState.Remove("Services");
            ModelState.Remove("TrainerAvailabilities");

            if (ModelState.IsValid)
            {
                // --- RESİM YÜKLEME İŞLEMİ BAŞLANGIÇ ---
                if (trainer.ImageUpload != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(trainer.ImageUpload.FileName);
                    string extension = Path.GetExtension(trainer.ImageUpload.FileName);

                    // Benzersiz isim oluştur (Aynı isimli resimler çakışmasın)
                    trainer.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                    string path = Path.Combine(wwwRootPath + "/images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await trainer.ImageUpload.CopyToAsync(fileStream);
                    }
                }
                // --- RESİM YÜKLEME İŞLEMİ BİTİŞ ---

=======
        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Specialization,ImageUpload,GymId")] Trainer trainer)
        {
            if (trainer.ImageUpload != null)
            {
                string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "images");
                string fileName = Guid.NewGuid().ToString() + "-" + trainer.ImageUpload.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await trainer.ImageUpload.CopyToAsync(fileStream);
                }
                trainer.ImageName = fileName;
            }

            ModelState.Remove("Gym");
            ModelState.Remove("Services");
            ModelState.Remove("ImageUpload");

            if (ModelState.IsValid)
            {
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
<<<<<<< HEAD

            // Hata varsa formu tekrar doldur
=======
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", trainer.GymId);
            return View(trainer);
        }

<<<<<<< HEAD
        // --- 5. DÜZENLEME SAYFASI ---
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

=======
        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return NotFound();
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", trainer.GymId);
            return View(trainer);
        }

<<<<<<< HEAD
        // --- 6. DÜZENLEME İŞLEMİ ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trainer trainer)
        {
            if (id != trainer.Id) return NotFound();

            // Validasyon düzeltmesi
            ModelState.Remove("Gym");
            ModelState.Remove("Services");
            ModelState.Remove("TrainerAvailabilities");

            if (ModelState.IsValid)
            {
                try
                {
                    // Eğer yeni resim yüklendiyse eskisini güncelle
                    if (trainer.ImageUpload != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(trainer.ImageUpload.FileName);
                        string extension = Path.GetExtension(trainer.ImageUpload.FileName);
                        trainer.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/images/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await trainer.ImageUpload.CopyToAsync(fileStream);
                        }
                    }
                    else
                    {
                        // Yeni resim yüklenmediyse eski resim adını koru
                        // (Veritabanından eski veriyi çekmemiz gerekebilir, basitlik için bu haliyle bırakıyorum
                        // ama burası için genellikle AsNoTracking ile eski veriyi çekip ImageName'i atarız.)
                    }

                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.Id)) return NotFound();
                    else throw;
                }
=======
        // POST: Edit (RESİM GÜNCELLEME MANTIĞI)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Specialization,ImageUpload,GymId,ImageName")] Trainer trainer)
        {
            if (id != trainer.Id) return NotFound();

            ModelState.Remove("Gym");
            ModelState.Remove("Services");
            ModelState.Remove("ImageUpload");

            if (ModelState.IsValid)
            {
                // Eğer yeni resim yüklendiyse
                if (trainer.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "images");

                    // 1. Eski resmi sil
                    if (!string.IsNullOrEmpty(trainer.ImageName))
                    {
                        string oldPath = Path.Combine(uploadDir, trainer.ImageName);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    // 2. Yeni resmi kaydet
                    string fileName = Guid.NewGuid().ToString() + "-" + trainer.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await trainer.ImageUpload.CopyToAsync(fileStream);
                    }
                    trainer.ImageName = fileName;
                }

                _context.Update(trainer);
                await _context.SaveChangesAsync();
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
                return RedirectToAction(nameof(Index));
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", trainer.GymId);
            return View(trainer);
        }

<<<<<<< HEAD
        // --- 7. SİLME EKRANI ---
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.Services)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // --- 8. SİLME İŞLEMİ (TEMİZLİK YAPAN KOD) ---
=======
        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null) return NotFound();
            return View(trainer);
        }

        // POST: Delete (RESİM SİLME MANTIĞI)
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
<<<<<<< HEAD
            var trainer = await _context.Trainers
                .Include(t => t.Services)
                .Include(t => t.TrainerAvailabilities)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainer != null)
            {
                // Randevuları temizle
                var appointments = _context.Appointments.Where(a => a.TrainerId == id);
                _context.Appointments.RemoveRange(appointments);

                // Takvimi temizle
                if (trainer.TrainerAvailabilities != null)
                {
                    _context.TrainerAvailabilities.RemoveRange(trainer.TrainerAvailabilities);
                }

                // Hizmetlerden hocayı düşür
                if (trainer.Services != null)
                {
                    foreach (var service in trainer.Services)
                    {
                        service.TrainerId = null;
                    }
                }

                // Resmi klasörden de silebilirsin (Opsiyonel)
=======
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                // Resmi klasörden de sil
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
                if (!string.IsNullOrEmpty(trainer.ImageName))
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", trainer.ImageName);
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
                }

                _context.Trainers.Remove(trainer);
<<<<<<< HEAD
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
=======
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.Services) // Hangi dersleri veriyor görelim
                .ThenInclude(s => s.ServiceCategory)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();
            return View(trainer);
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        }
    }
}