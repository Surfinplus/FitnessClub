using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
<<<<<<< HEAD
using Microsoft.AspNetCore.Hosting; // Dosya iþlemleri için gerekli
=======
using System.Linq;
using System.Threading.Tasks;
>>>>>>> 32b5aca96bd0f9c2f69d4f0a98154b5a24638139

namespace Fitnesclubplus.Controllers
{
    public class GymsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; // Resim yolu için eklendi

        // Constructor güncellendi: hostEnvironment parametresi eklendi
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
<<<<<<< HEAD
        // DÝKKAT: Bind kýsmýna 'ImageUpload' eklendi. Bu olmadan resim null gelir.
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Description,ImageUpload")] Gym gym, string acilisSaati, string kapanisSaati)
        {
            // 1. SAAT BÝRLEÞTÝRME (Senin kodun)
=======
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Description")] Gym gym, string acilisSaati, string kapanisSaati)
        {
            // 1. SAAT BÝRLEÞTÝRME
>>>>>>> 32b5aca96bd0f9c2f69d4f0a98154b5a24638139
            if (!string.IsNullOrEmpty(acilisSaati) && !string.IsNullOrEmpty(kapanisSaati))
            {
                gym.OpeningHours = $"{acilisSaati} - {kapanisSaati}";
            }
            else
            {
                gym.OpeningHours = "Belirtilmedi";
            }

<<<<<<< HEAD
            // 2. RESÝM YÜKLEME ÝÞLEMÝ (YENÝ EKLENEN KISIM)
            if (gym.ImageUpload != null)
            {
                // Resimlerin kaydedileceði klasör yolu: wwwroot/images
                string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "images");

                // Benzersiz dosya adý oluþtur (örn: guid-resim.jpg)
                string fileName = Guid.NewGuid().ToString() + "-" + gym.ImageUpload.FileName;
                string filePath = Path.Combine(uploadDir, fileName);

                // Dosyayý klasöre kopyala
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await gym.ImageUpload.CopyToAsync(fileStream);
                }

                // Veritabanýna dosya adýný kaydet
                gym.ImageName = fileName;
            }

            // 3. HATALARI SÝL (Senin kodun + ImageUpload hatasýný da siliyoruz)
            ModelState.Remove("OpeningHours");
            ModelState.Remove("Description");
            ModelState.Remove("Services");
            ModelState.Remove("Appointments");
            ModelState.Remove("ImageUpload"); // Model state validation hatasý vermesin diye

            // 4. KAYIT ÝÞLEMÝ
=======
            // 2. HATALARI SÝL (ÇÖZÜM BURADA)
            // Bu satýrlar veritabaný iliþkilerinden kaynaklanan "Zorunlu Alan" hatalarýný kaldýrýr.
            ModelState.Remove("OpeningHours");
            ModelState.Remove("Description");
            ModelState.Remove("Services");      // <--- EKLENDÝ
            ModelState.Remove("Appointments");  // <--- EKLENDÝ

            // 3. KAYIT ÝÞLEMÝ
>>>>>>> 32b5aca96bd0f9c2f69d4f0a98154b5a24638139
            if (ModelState.IsValid)
            {
                _context.Add(gym);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // --- HATA OLURSA KONSOLA YAZ ---
            Debug.WriteLine("---------------- HATA RAPORU ----------------");
            foreach (var item in ModelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    Debug.WriteLine($"ALAN: {item.Key} - HATA: {error.ErrorMessage}");
                }
            }
            Debug.WriteLine("---------------------------------------------");

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
<<<<<<< HEAD
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,OpeningHours,Description,ImageName")] Gym gym)
=======
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,OpeningHours,Description")] Gym gym)
>>>>>>> 32b5aca96bd0f9c2f69d4f0a98154b5a24638139
        {
            if (id != gym.Id)
            {
                return NotFound();
            }

<<<<<<< HEAD
            ModelState.Remove("Services");
            ModelState.Remove("Appointments");
            ModelState.Remove("ImageUpload");
=======
            // Edit iþlemi sýrasýnda da bu hatalarý almamak için buraya da ekledik:
            ModelState.Remove("Services");      // <--- EKLENDÝ
            ModelState.Remove("Appointments");  // <--- EKLENDÝ
>>>>>>> 32b5aca96bd0f9c2f69d4f0a98154b5a24638139

            if (ModelState.IsValid)
            {
                try
                {
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