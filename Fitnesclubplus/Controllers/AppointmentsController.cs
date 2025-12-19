using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;

namespace Fitnesclubplus.Controllers
{
    [Authorize(Roles = "Admin")] // Şimdilik sadece Adminler yönetebilsin
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            // Randevuları getirirken hangi hizmete ait olduğunu da (Include) getiriyoruz.
            var applicationDbContext = _context.Appointments.Include(a => a.Service);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Service) // Detaylarda hizmet adını görmek için
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            // Dropdown için Hizmetleri (Services) listeliyoruz
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "ServiceName");
            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppointmentDate,Status,UserId,ServiceId")] Appointment appointment)
        {
            // --- VALIDATION DÜZELTMESİ ---
            // Service nesnesi formdan null gelir, sadece ID gelir. Hata vermesin diye siliyoruz.
            ModelState.Remove("Service");

            // Kullanıcı (User) henüz giriş yapmamışsa veya admin elle ekliyorsa UserId null olabilir.
            // Bu yüzden UserId kontrolünü de kaldırıyoruz (şimdilik).
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                // Oluşturulma tarihini otomatik atayalım
                appointment.CreatedDate = DateTime.Now;

                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Hata olursa listeyi tekrar doldur
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppointmentDate,CreatedDate,Status,UserId,ServiceId")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Service");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}