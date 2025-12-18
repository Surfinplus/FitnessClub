using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting; // Dosya yollarý (wwwroot) için gerekli
using System.IO; // <-- BU EKLENDÝ! (Path ve FileStream hatalarýný çözer)

namespace Fitnesclubplus.Controllers
{
    public class GymsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public GymsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Gyms
        public async Task<IActionResult> Index()
        {
            return View(await _context.Gyms.ToListAsync());
        }

        // GET: Gyms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gym = await _context.Gyms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gym == null)
            {
                return NotFound();
            }

            return View(gym);
        }

        // GET: Gyms/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gyms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

            if (ModelState.IsValid)
            {
                _context.Add(gym);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gym);
        }

        // GET: Gyms/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gym = await _context.Gyms.FindAsync(id);
            if (gym == null)
            {
                return NotFound();
            }
            return View(gym);
        }

        // POST: Gyms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
                return RedirectToAction(nameof(Index));
            }
            return View(gym);
        }

        // GET: Gyms/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gym = await _context.Gyms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gym == null)
            {
                return NotFound();
            }

            return View(gym);
        }

        // POST: Gyms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gym = await _context.Gyms.FindAsync(id);
            if (gym != null)
            {
                _context.Gyms.Remove(gym);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymExists(int id)
        {
            return _context.Gyms.Any(e => e.Id == id);
        }
    }
}