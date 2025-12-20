using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;

namespace Fitnesclubplus.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Services (Listeleme)
        public async Task<IActionResult> Index()
        {
            var services = _context.Services
                .Include(s => s.Gym)
                .Include(s => s.ServiceCategory) // Kategori adýný görmek için
                .Include(s => s.Trainer);        // Eðitmen adýný görmek için
            return View(await services.ToListAsync());
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services
                .Include(s => s.Gym)
                .Include(s => s.ServiceCategory)
                .Include(s => s.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (service == null) return NotFound();

            return View(service);
        }

        // GET: Services/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Dropdown'larý dolduruyoruz
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name");
            ViewData["ServiceCategoryId"] = new SelectList(_context.ServiceCategories, "Id", "Name");
            // Trainer listesini boþ gönderiyoruz veya tümünü göndersek bile JS bunu ezecek.
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName");
            return View();
        }

        // POST: Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,ServiceCategoryId,TrainerId,DurationMinutes,Price,GymId,Description")] Service service)
        {
            // Validation temizliði
            ModelState.Remove("Gym");
            ModelState.Remove("ServiceCategory");
            ModelState.Remove("Trainer");
            ModelState.Remove("Description");

            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Hata olursa listeleri tekrar doldur
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", service.GymId);
            ViewData["ServiceCategoryId"] = new SelectList(_context.ServiceCategories, "Id", "Name", service.ServiceCategoryId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", service.TrainerId);
            return View(service);
        }

        // GET: Services/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", service.GymId);
            ViewData["ServiceCategoryId"] = new SelectList(_context.ServiceCategories, "Id", "Name", service.ServiceCategoryId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", service.TrainerId);
            return View(service);
        }

        // POST: Services/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ServiceCategoryId,TrainerId,DurationMinutes,Price,GymId,Description")] Service service)
        {
            if (id != service.Id) return NotFound();

            ModelState.Remove("Gym");
            ModelState.Remove("ServiceCategory");
            ModelState.Remove("Trainer");
            ModelState.Remove("Description");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", service.GymId);
            ViewData["ServiceCategoryId"] = new SelectList(_context.ServiceCategories, "Id", "Name", service.ServiceCategoryId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", service.TrainerId);
            return View(service);
        }

        // GET: Services/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services
                .Include(s => s.Gym)
                .Include(s => s.ServiceCategory)
                .Include(s => s.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (service == null) return NotFound();

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }

        // --- YENÝ EKLENEN AJAX METODU ---
        // Bu metod, View tarafýndaki JavaScript tarafýndan çaðrýlýr.
        // Seçilen Salon ID'sine göre o salondaki eðitmenleri JSON olarak döndürür.
        [HttpGet]
        public JsonResult GetTrainersByGym(int gymId)
        {
            var trainers = _context.Trainers
                .Where(t => t.GymId == gymId)
                .Select(t => new {
                    id = t.Id,
                    fullName = t.FullName
                })
                .ToList();

            return Json(trainers);
        }
    }
}