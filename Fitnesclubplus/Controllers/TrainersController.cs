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
        private readonly IWebHostEnvironment _hostEnvironment;

        public TrainersController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            var trainers = _context.Trainers.Include(t => t.Gym);
            return View(await trainers.ToListAsync());
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name");
            return View();
        }

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
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", trainer.GymId);
            return View(trainer);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null) return NotFound();
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", trainer.GymId);
            return View(trainer);
        }

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
                return RedirectToAction(nameof(Index));
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", trainer.GymId);
            return View(trainer);
        }

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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                // Resmi klasörden de sil
                if (!string.IsNullOrEmpty(trainer.ImageName))
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", trainer.ImageName);
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
                }

                _context.Trainers.Remove(trainer);
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
        }
    }
}