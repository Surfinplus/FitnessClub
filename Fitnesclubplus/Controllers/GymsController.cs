using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;

namespace Fitnesclubplus.Controllers
{
    public class GymsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GymsController(ApplicationDbContext context)
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
        public async Task<IActionResult> Create([Bind("Id,Name,Address,OpeningHours,ImageName,ImageUpload")] Gym gym)
        {
            // Resim yükleme kodlarýn buraya gelecek (Þimdilik temel kaydetme)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,OpeningHours,ImageName")] Gym gym)
        {
            if (id != gym.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(gym);
                await _context.SaveChangesAsync();
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
            if (gym != null) _context.Gyms.Remove(gym);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}