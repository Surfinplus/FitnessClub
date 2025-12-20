using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitnesclubplus.Data;
using Fitnesclubplus.Models;

namespace Fitnesclubplus.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrainerApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TrainerApi/GetSchedule/5
        // ŞARTNAME: Belirli bir tarihte uygun antrenörleri getirme (LINQ Kullanımı)
        [HttpGet("GetSchedule/{trainerId}")]
        public async Task<IActionResult> GetSchedule(int trainerId)
        {
            // LINQ SORGUSU BAŞLIYOR
            var schedule = await _context.TrainerAvailabilities
                .Where(t => t.TrainerId == trainerId) // 1. Filtre: Sadece bu hoca
                .Where(t => t.IsGeneral == true || t.Date >= DateTime.Today) // 2. Filtre: Geçmiş tarihleri ele
                .OrderByDescending(t => t.IsGeneral) // 3. Sıralama: Önce genel olanlar
                .ThenBy(t => t.Date) // Sonra tarihe göre
                .Select(t => new
                {
                    // Sadece ihtiyacımız olan veriyi çekiyoruz (Performans)
                    IsGeneral = t.IsGeneral,
                    DateText = t.Date.HasValue ? t.Date.Value.ToString("dd.MM.yyyy") : "",
                    DayName = t.Date.HasValue ? t.Date.Value.ToString("dddd", new System.Globalization.CultureInfo("tr-TR")) : "Her Gün",
                    Start = t.StartTime.ToString(@"hh\:mm"),
                    End = t.EndTime.ToString(@"hh\:mm")
                })
                .ToListAsync();

            return Ok(schedule);
        }
    }
}