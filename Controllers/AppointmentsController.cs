using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models;
using FitnessCenter.Models.ViewModels;
using System.Globalization;

namespace FitnessCenter.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            // [Authorize] attribute ensures user is authenticated
            var currentUser = await _userManager.GetUserAsync(User);
            
            // If GetUserAsync returns null, it means the user in the cookie doesn't exist in DB
            // This should be rare, but handle it gracefully
            if (currentUser == null)
            {
                // Sign out the invalid session and redirect to login
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                TempData["Error"] = "Oturum geçersiz. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Account");
            }

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            IQueryable<Appointment> appointmentsQuery = _context.Appointments
                .Include(a => a.Member)
                    .ThenInclude(m => m.ApplicationUser)
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Include(a => a.Gym);

            if (isAdmin)
            {
                // Admin sadece kendi spor salonunun randevularını görür
                if (currentUser.GymId == null)
                {
                    TempData["Error"] = "Spor salonunuz atanmamış.";
                    return RedirectToAction("Index", "Home");
                }
                appointmentsQuery = appointmentsQuery.Where(a => a.GymId == currentUser.GymId);
            }
            else
            {
                // Member can only see their own appointments
                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.IdentityUserId == currentUser.Id);
                
                if (member == null)
                {
                    return RedirectToAction("Create", "Members");
                }

                appointmentsQuery = appointmentsQuery.Where(a => a.MemberId == member.Id);
            }

            var appointments = await appointmentsQuery
                .OrderByDescending(a => a.StartDateTime)
                .ToListAsync();

            ViewBag.IsAdmin = isAdmin;
            return View(appointments);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Member)
                    .ThenInclude(m => m.ApplicationUser)
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Include(a => a.Gym)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            // Check authorization
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            if (!isAdmin)
            {
                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.IdentityUserId == currentUser.Id);
                
                if (member == null || appointment.MemberId != member.Id)
                {
                    return Forbid();
                }
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if member exists
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.IdentityUserId == currentUser.Id);
            
            if (member == null)
            {
                TempData["Error"] = "Lütfen önce üye profilinizi tamamlayın.";
                return RedirectToAction("Create", "Members");
            }

            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            
            // Tüm antrenörleri başlangıçta göster (hizmet seçilince JavaScript ile filtreleyeceğiz)
            ViewData["TrainerId"] = new SelectList(_context.Trainers.Select(t => new { t.Id, Name = $"{t.FirstName} {t.LastName}" }), "Id", "Name");
            
            // Antrenör-Hizmet ilişkilerini JSON olarak gönder (JavaScript için)
            var trainerServices = await _context.TrainerServices
                .Select(ts => new { TrainerId = ts.TrainerId, ServiceId = ts.ServiceId })
                .ToListAsync();
            ViewBag.TrainerServicesJson = System.Text.Json.JsonSerializer.Serialize(trainerServices);
            
            // Tüm antrenörleri JSON olarak gönder
            var allTrainers = await _context.Trainers
                .Select(t => new { Id = t.Id, Name = $"{t.FirstName} {t.LastName}" })
                .ToListAsync();
            ViewBag.AllTrainersJson = System.Text.Json.JsonSerializer.Serialize(allTrainers);
            
            return View(new CreateAppointmentViewModel { Date = DateTime.Today });
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create(CreateAppointmentViewModel viewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.IdentityUserId == currentUser.Id);
            
            if (member == null)
            {
                ModelState.AddModelError("", "Please complete your member profile first.");
                ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", viewModel.GymId);
                ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", viewModel.ServiceId);
                ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FirstName", viewModel.TrainerId);
                return View(viewModel);
            }

            // Load related entities
            var service = await _context.Services.FindAsync(viewModel.ServiceId);
            var trainer = await _context.Trainers.FindAsync(viewModel.TrainerId);
            var gym = await _context.Gyms.FindAsync(viewModel.GymId);

            if (service == null || trainer == null || gym == null)
            {
                ModelState.AddModelError("", "Geçersiz seçim. Lütfen tekrar deneyin.");
                ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", viewModel.GymId);
                ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", viewModel.ServiceId);
                ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FirstName", viewModel.TrainerId);
                return View(viewModel);
            }

            // Build StartDateTime and EndDateTime
            var startDateTime = viewModel.Date.Date.Add(viewModel.StartTime);
            var endDateTime = startDateTime.AddMinutes(service.DurationInMinutes);

            // Validation: Check for conflicts
            var validationErrors = await ValidateAppointmentAsync(
                trainerId: viewModel.TrainerId,
                memberId: member.Id,
                startDateTime: startDateTime,
                endDateTime: endDateTime,
                dayOfWeek: (int)startDateTime.DayOfWeek,
                startTime: viewModel.StartTime);

            foreach (var error in validationErrors)
            {
                ModelState.AddModelError("", error);
            }

            if (!ModelState.IsValid)
            {
                ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", viewModel.GymId);
                ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", viewModel.ServiceId);
                ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FirstName", viewModel.TrainerId);
                return View(viewModel);
            }

            // Create appointment
            var appointment = new Appointment
            {
                MemberId = member.Id,
                TrainerId = viewModel.TrainerId,
                ServiceId = viewModel.ServiceId,
                GymId = viewModel.GymId,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                Status = AppointmentStatus.Pending,
                PriceAtBooking = service.Price,
                Notes = viewModel.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Add(appointment);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Randevu başarıyla oluşturuldu! Onay bekliyor.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Appointments/ChangeStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(int id, AppointmentStatus status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = status;
            await _context.SaveChangesAsync();

            var statusText = status == AppointmentStatus.Approved ? "Onaylandı" : 
                           status == AppointmentStatus.Cancelled ? "İptal Edildi" : "Beklemede";
            TempData["Success"] = $"Randevu durumu '{statusText}' olarak güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Appointments/Cancel/5 - Üyeler kendi randevularını iptal edebilir
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Cancel(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.IdentityUserId == currentUser.Id);
            
            if (member == null)
            {
                TempData["Error"] = "Üye profiliniz bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.MemberId == member.Id);

            if (appointment == null)
            {
                TempData["Error"] = "Randevu bulunamadı veya bu randevuyu iptal etme yetkiniz yok.";
                return RedirectToAction(nameof(Index));
            }

            // Sadece Beklemede veya Onaylandı durumundaki randevular iptal edilebilir
            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                TempData["Error"] = "Bu randevu zaten iptal edilmiş.";
                return RedirectToAction(nameof(Index));
            }

            appointment.Status = AppointmentStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Randevunuz başarıyla iptal edildi.";
            return RedirectToAction(nameof(Index));
        }

        // Helper method for validation
        private async Task<List<string>> ValidateAppointmentAsync(
            int trainerId, 
            int memberId, 
            DateTime startDateTime, 
            DateTime endDateTime,
            int dayOfWeek,
            TimeSpan startTime)
        {
            var errors = new List<string>();

            // 1. Check for trainer conflicts
            var trainerConflict = await _context.Appointments
                .Where(a => a.TrainerId == trainerId 
                    && a.Status != AppointmentStatus.Cancelled
                    && ((a.StartDateTime < endDateTime && a.EndDateTime > startDateTime)))
                .AnyAsync();

            if (trainerConflict)
            {
                errors.Add("Antrenörün bu saatte zaten bir randevusu var.");
            }

            // 2. Check for member conflicts
            var memberConflict = await _context.Appointments
                .Where(a => a.MemberId == memberId 
                    && a.Status != AppointmentStatus.Cancelled
                    && ((a.StartDateTime < endDateTime && a.EndDateTime > startDateTime)))
                .AnyAsync();

            if (memberConflict)
            {
                errors.Add("Bu saatte zaten bir randevunuz var.");
            }

            // 3. Check trainer availability
            var trainerAvailability = await _context.TrainerAvailabilities
                .Where(ta => ta.TrainerId == trainerId && ta.DayOfWeek == dayOfWeek)
                .ToListAsync();

            if (!trainerAvailability.Any())
            {
                errors.Add("Antrenör bu gün müsait değil.");
            }
            else
            {
                var isAvailable = trainerAvailability.Any(ta =>
                {
                    var availabilityStart = TimeSpan.Parse(ta.StartTime);
                    var availabilityEnd = TimeSpan.Parse(ta.EndTime);
                    return startTime >= availabilityStart && endDateTime.TimeOfDay <= availabilityEnd;
                });

                if (!isAvailable)
                {
                    errors.Add("Seçilen saat antrenörün müsaitlik saatleri dışında.");
                }
            }

            return errors;
        }
    }
}

