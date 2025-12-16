using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Fitnesclubplus.Controllers
{
    public class GymsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GymsController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Description")] Gym gym, string acilisSaati, string kapanisSaati)
        {
            // 1. SAAT BÝRLEÞTÝRME
            if (!string.IsNullOrEmpty(acilisSaati) && !string.IsNullOrEmpty(kapanisSaati))
            {
                gym.OpeningHours = $"{acilisSaati} - {kapanisSaati}";
            }
            else
            {
                gym.OpeningHours = "Belirtilmedi";
            }

            // 2. HATALARI SÝL (ÇÖZÜM BURADA)
            // Bu satýrlar veritabaný iliþkilerinden kaynaklanan "Zorunlu Alan" hatalarýný kaldýrýr.
            ModelState.Remove("OpeningHours");
            ModelState.Remove("Description");
            ModelState.Remove("Services");      // <--- EKLENDÝ
            ModelState.Remove("Appointments");  // <--- EKLENDÝ

            // 3. KAYIT ÝÞLEMÝ
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,OpeningHours,Description")] Gym gym)
        {
            if (id != gym.Id)
            {
                return NotFound();
            }

            // Edit iþlemi sýrasýnda da bu hatalarý almamak için buraya da ekledik:
            ModelState.Remove("Services");      // <--- EKLENDÝ
            ModelState.Remove("Appointments");  // <--- EKLENDÝ

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