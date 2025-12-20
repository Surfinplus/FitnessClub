using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;
<<<<<<< HEAD
=======
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting; // Dosya yollarý (wwwroot) için gerekli
using System.IO; // <-- BU EKLENDÝ! (Path ve FileStream hatalarýný çözer)
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138

namespace Fitnesclubplus.Controllers
{
    public class GymsController : Controller
    {
        private readonly ApplicationDbContext _context;
<<<<<<< HEAD

        public GymsController(ApplicationDbContext context)
=======
        private readonly IWebHostEnvironment _hostEnvironment;

        public GymsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
        {
            _context = context;
        }

        // 1. LÝSTELEME (HERKES GÖRÜR)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Gyms.ToListAsync());
        }

        // 2. DETAY GÖRÜNTÜLEME (HERKES GÖRÜR - ZENGÝN ÝÇERÝK)
        // 2. DETAY GÖRÜNTÜLEME
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var gym = await _context.Gyms
                .Include(g => g.Trainers)
                    .ThenInclude(t => t.TrainerAvailabilities) // <--- KRÝTÝK NOKTA: Saatleri de getir
                .Include(g => g.Services)
                    .ThenInclude(s => s.ServiceCategory)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gym == null) return NotFound();

            return View(gym);
        }

        // --- SADECE ADMÝN GÝREBÝLÝR (KORUMALI ALAN) ---

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
<<<<<<< HEAD
        public async Task<IActionResult> Create([Bind("Id,Name,Address,OpeningHours,ImageName,ImageUpload")] Gym gym)
        {
            // Resim yükleme kodlarýn buraya gelecek (Þimdilik temel kaydetme)
=======
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Description,ImageUpload")] Gym gym, string acilisSaati, string kapanisSaati)
        {
            // Saat Birleþtirme
            if (!string.IsNullOrEmpty(acilisSaati) && !string.IsNullOrEmpty(kapanisSaati))
            {
                gym.OpeningHours = $"{acilisSaati} - {kapanisSaati}";
            }
            else
            {
                gym.OpeningHours = "Belirtilmedi";
            }

            // Resim Kaydetme Ýþlemi
            if (gym.ImageUpload != null)
            {
                string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "images");
                string fileName = Guid.NewGuid().ToString() + "-" + gym.ImageUpload.FileName;
                string filePath = Path.Combine(uploadDir, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await gym.ImageUpload.CopyToAsync(fileStream);
                }
                gym.ImageName = fileName;
            }

            // Hata vermemesi için doðrulama temizliði
            ModelState.Remove("OpeningHours");
            ModelState.Remove("Description");
            ModelState.Remove("Services");
            ModelState.Remove("ImageUpload");

>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
            if (ModelState.IsValid)
            {
                _context.Add(gym);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gym);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var gym = await _context.Gyms.FindAsync(id);
            if (gym == null) return NotFound();
            return View(gym);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
<<<<<<< HEAD
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,OpeningHours,ImageName")] Gym gym)
        {
            if (id != gym.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(gym);
                await _context.SaveChangesAsync();
=======
        // Bind içine ImageUpload ve ImageName eklendi
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,OpeningHours,Description,ImageName,ImageUpload")] Gym gym)
        {
            if (id != gym.Id)
            {
                return NotFound();
            }

            // Doðrulama hatalarýný temizle
            ModelState.Remove("Services");
            ModelState.Remove("ImageUpload");

            if (ModelState.IsValid)
            {
                try
                {
                    // --- RESÝM GÜNCELLEME MANTIÐI ---
                    if (gym.ImageUpload != null)
                    {
                        // 1. Yeni resim seçildiyse kaydet
                        string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "images");
                        string fileName = Guid.NewGuid().ToString() + "-" + gym.ImageUpload.FileName;
                        string filePath = Path.Combine(uploadDir, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await gym.ImageUpload.CopyToAsync(fileStream);
                        }
                        // 2. Yeni dosya adýný modele yaz
                        gym.ImageName = fileName;
                    }
                    // Eðer resim seçilmediyse, View'den gelen eski 'ImageName' zaten korunur.

                    _context.Update(gym);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymExists(gym.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
                return RedirectToAction(nameof(Index));
            }
            return View(gym);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var gym = await _context.Gyms.FirstOrDefaultAsync(m => m.Id == id);
            if (gym == null) return NotFound();
            return View(gym);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gym = await _context.Gyms.FindAsync(id);
<<<<<<< HEAD
            if (gym != null) _context.Gyms.Remove(gym);
=======
            if (gym != null)
            {
                _context.Gyms.Remove(gym);
            }
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}