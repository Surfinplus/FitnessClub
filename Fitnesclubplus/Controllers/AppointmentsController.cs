using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Fitnesclubplus.Controllers
{
    [Authorize] // Üye girişi zorunlu
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- 1. LİSTELEME VE YÖNETİM ---
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var appointmentsQuery = _context.Appointments
                .Include(a => a.Service)
                .ThenInclude(s => s.ServiceCategory)
                .Include(a => a.Trainer)
                .Include(a => a.User)
                .AsQueryable();

            if (!isAdmin)
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.UserId == userId);
            }

            var appointments = await appointmentsQuery.OrderByDescending(a => a.AppointmentDate).ToListAsync();
            return View(appointments);
        }

        // --- 2. ONAYLA (Admin) ---
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Confirmed;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // --- 3. REDDET (Admin) ---
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // --- 4. SİLME ---
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var appointment = await _context.Appointments.FindAsync(id);
            var userId = _userManager.GetUserId(User);

            if (appointment != null && (User.IsInRole("Admin") || appointment.UserId == userId))
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // --- 5. YENİ RANDEVU SAYFASI ---
        public IActionResult Create()
        {
            var services = _context.Services
                .Include(s => s.ServiceCategory)
                .Include(s => s.Trainer)
                .Select(s => new
                {
                    Id = s.Id,
                    Text = $"{s.ServiceCategory.Name} - {(s.Trainer != null ? s.Trainer.FullName : "Genel")} ({s.Price:C0})"
                })
                .ToList();

            ViewData["ServiceId"] = new SelectList(services, "Id", "Text");
            return View();
        }

        
        [HttpGet]
        public async Task<JsonResult> GetServiceDetails(int serviceId)
        {
            var service = await _context.Services
                .Include(s => s.Trainer)
                .FirstOrDefaultAsync(s => s.Id == serviceId);

            if (service == null) return Json(null);

            return Json(new
            {
                trainerId = service.TrainerId,
                trainerName = service.Trainer != null ? service.Trainer.FullName : "Atanmamış",
                price = service.Price,
                duration = service.DurationMinutes
            });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceId,AppointmentDate,TrainerId")] Appointment appointment)
        {
            // Kullanıcı Bilgileri
            var currentUserId = _userManager.GetUserId(User);
            appointment.UserId = currentUserId;
            appointment.Status = AppointmentStatus.Pending;
            appointment.CreatedDate = DateTime.Now;

            // --- ADIM 1: TEMEL KONTROLLER ---
            if (appointment.AppointmentDate < DateTime.Now)
            {
                ModelState.AddModelError("AppointmentDate", "Geçmiş bir tarihe randevu alamazsınız.");
            }

            // Hizmetin Süresini Öğren
            var service = await _context.Services.FindAsync(appointment.ServiceId);
            if (service == null)
            {
                ModelState.AddModelError("", "Hizmet bulunamadı.");
                return ReloadView(appointment);
            }

            // Talep Edilen Zaman Aralığı
            var requestStart = appointment.AppointmentDate;
            var requestEnd = requestStart.AddMinutes(service.DurationMinutes);

            // --- ADIM 2: HOCANIN ÇALIŞMA SAATİ KONTROLÜ ---
            if (appointment.TrainerId != 0)
            {
                var requestDateOnly = requestStart.Date;
                var requestTimeOnlyStart = requestStart.TimeOfDay;
                var requestTimeOnlyEnd = requestEnd.TimeOfDay;

                var isWorking = await _context.TrainerAvailabilities
                    .Where(t => t.TrainerId == appointment.TrainerId)
                    .Where(t => (t.IsGeneral == true) || (t.Date == requestDateOnly))
                    .AnyAsync(t => requestTimeOnlyStart >= t.StartTime && requestTimeOnlyEnd <= t.EndTime);

                if (!isWorking)
                {
                    ModelState.AddModelError("AppointmentDate", "Seçtiğiniz saatlerde eğitmen çalışmıyor (Mesai saatleri dışında).");
                    return ReloadView(appointment);
                }
            }

            // --- ADIM 3: MÜŞTERİ ÇAKIŞMA KONTROLÜ (HATA BURADAYDI, DÜZELDİ) ---
            var userConflict = await _context.Appointments
                .Include(a => a.Service)
                .ThenInclude(s => s.ServiceCategory) // <--- KRİTİK DÜZELTME: Kategori adını çekmek için bu şart!
                .Where(a => a.UserId == currentUserId)
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .Where(a => a.AppointmentDate < requestEnd &&
                            a.AppointmentDate.AddMinutes(a.Service.DurationMinutes) > requestStart)
                .FirstOrDefaultAsync();

            if (userConflict != null)
            {
                // Güvenli isim alma (Null check)
                string conflictServiceName = userConflict.Service?.ServiceCategory?.Name ?? "Hizmet";

                ModelState.AddModelError("", $"Bu saat aralığında zaten başka bir randevunuz var! ({userConflict.AppointmentDate:HH:mm} - {conflictServiceName})");
                return ReloadView(appointment);
            }

            // --- ADIM 4: HOCA ÇAKIŞMA KONTROLÜ ---
            if (appointment.TrainerId != 0)
            {
                var trainerConflict = await _context.Appointments
                    .Include(a => a.Service)
                    .Where(a => a.TrainerId == appointment.TrainerId)
                    .Where(a => a.Status != AppointmentStatus.Cancelled)
                    .Where(a => a.AppointmentDate < requestEnd &&
                                a.AppointmentDate.AddMinutes(a.Service.DurationMinutes) > requestStart)
                    .FirstOrDefaultAsync();

                if (trainerConflict != null)
                {
                    ModelState.AddModelError("AppointmentDate", "Eğitmen bu saat aralığında başka bir üyeye hizmet veriyor (Dolu).");
                    return ReloadView(appointment);
                }
            }

            // --- HER ŞEY TEMİZSE KAYDET ---
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return ReloadView(appointment);
        }

        // Hata durumunda View'ı tekrar dolduran yardımcı metod
        private IActionResult ReloadView(Appointment appointment)
        {
            var services = _context.Services
                .Include(s => s.ServiceCategory)
                .Include(s => s.Trainer)
                .Select(s => new
                {
                    Id = s.Id,
                    Text = $"{s.ServiceCategory.Name} - {(s.Trainer != null ? s.Trainer.FullName : "Genel")} ({s.Price:C0})"
                })
                .ToList();
            ViewData["ServiceId"] = new SelectList(services, "Id", "Text", appointment.ServiceId);
            return View(appointment);
        }
    }
}