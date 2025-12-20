using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;

namespace Fitnesclubplus.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiceCategories.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ServiceCategory serviceCategory)
        {
            ModelState.Remove("Services");
            if (ModelState.IsValid)
            {
                _context.Add(serviceCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceCategory);
        }

        // --- YENİ EKLENEN KISIMLAR ---

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory == null) return NotFound();
            return View(serviceCategory);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ServiceCategory serviceCategory)
        {
            if (id != serviceCategory.Id) return NotFound();
            ModelState.Remove("Services");

            if (ModelState.IsValid)
            {
                _context.Update(serviceCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceCategory);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var serviceCategory = await _context.ServiceCategories.FirstOrDefaultAsync(m => m.Id == id);
            if (serviceCategory == null) return NotFound();
            return View(serviceCategory);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory != null) _context.ServiceCategories.Remove(serviceCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}