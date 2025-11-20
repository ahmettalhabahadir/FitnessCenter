using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models;

namespace FitnessCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GymsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GymsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Gyms
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            // Admin'in spor salonu var mı kontrol et
            if (currentUser?.GymId != null)
            {
                var gym = await _context.Gyms.FindAsync(currentUser.GymId);
                if (gym != null)
                {
                    ViewBag.HasGym = true;
                    return View(new List<Gym> { gym });
                }
                else
                {
                    // Spor salonu silinmiş, admin'in GymId'sini temizle
                    currentUser.GymId = null;
                    await _userManager.UpdateAsync(currentUser);
                }
            }
            
            // Admin'in spor salonu yok
            ViewBag.HasGym = false;
            return View(new List<Gym>());
        }

        // GET: Gyms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.GymId != id)
            {
                return Forbid(); // Admin sadece kendi spor salonunu görebilir
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
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            // Eğer admin'in zaten bir spor salonu varsa, yeni oluşturamaz
            if (currentUser?.GymId != null)
            {
                TempData["Error"] = "Zaten bir spor salonunuz var. Yeni spor salonu oluşturamazsınız.";
                return RedirectToAction(nameof(Index));
            }
            
            return View();
        }

        // POST: Gyms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Phone,Email,OpeningTime,ClosingTime,Description,WorkingHours")] Gym gym)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            // Eğer admin'in zaten bir spor salonu varsa, yeni oluşturamaz
            if (currentUser?.GymId != null)
            {
                TempData["Error"] = "Zaten bir spor salonunuz var. Yeni spor salonu oluşturamazsınız.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                _context.Add(gym);
                await _context.SaveChangesAsync();
                
                // Admin'e yeni oluşturulan spor salonunu ata
                if (currentUser != null)
                {
                    currentUser.GymId = gym.Id;
                    await _userManager.UpdateAsync(currentUser);
                }
                
                TempData["Success"] = "Spor salonu başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(gym);
        }

        // GET: Gyms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.GymId != id)
            {
                return Forbid(); // Admin sadece kendi spor salonunu düzenleyebilir
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Phone,Email,OpeningTime,ClosingTime,Description,WorkingHours")] Gym gym)
        {
            if (id != gym.Id)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.GymId != id)
            {
                return Forbid(); // Admin sadece kendi spor salonunu silebilir
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.GymId != id)
            {
                return Forbid(); // Admin sadece kendi spor salonunu silebilir
            }

            var gym = await _context.Gyms.FindAsync(id);
            if (gym != null)
            {
                _context.Gyms.Remove(gym);
                await _context.SaveChangesAsync();
                
                // Admin'in GymId'sini temizle
                currentUser.GymId = null;
                await _userManager.UpdateAsync(currentUser);
                
                TempData["Success"] = "Spor salonu başarıyla silindi. Yeni spor salonu oluşturabilirsiniz.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GymExists(int id)
        {
            return _context.Gyms.Any(e => e.Id == id);
        }
    }
}

