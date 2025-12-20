<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fitnesclubplus.Data;
using Fitnesclubplus.Models;
using Microsoft.AspNetCore.Authorization;
<<<<<<< HEAD
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

        // --- 6. AJAX DETAY ---
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

        // --- 7. KAYDETME (DÜZELTİLMİŞ ÇAKIŞMA KONTROLÜ) ---
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
=======

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

>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

<<<<<<< HEAD
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
=======
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
>>>>>>> ced9dad4428e227e9f010d3675992cbbe43be138
    }
}