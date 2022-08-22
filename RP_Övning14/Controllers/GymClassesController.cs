using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RP_Övning14.Data;
using RP_Övning14.Models;

namespace RP_Övning14.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GymClassesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: GymClasses
        public async Task<IActionResult> Index()
        {
            //
              return _context.GymClasses != null ? 
                          View(await _context.GymClasses.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.GymClasses'  is null.");
        }

        // GET: GymClasses/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            gymClass.AttendingMembers = new List<ApplicationUserGymClass>();
            if (!ModelState.IsValid)
            {
                gymClass.Duration = gymClass.Duration / 24;
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GymClasses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GymClasses'  is null.");
            }
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClasses.Remove(gymClass);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
          return (_context.GymClasses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [Authorize]
        public async Task<IActionResult> BookingToogel(int? id)
        {
            if(id == null) return NotFound();

            GymClass GC = _context.GymClasses.Where(e => e.Id == id).FirstOrDefault();
            if (GC == null) return NotFound();

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Index));

            if (GC.AttendingMembers.Any(e => e.ApplicationUserId == user.Id))
            {      
                ApplicationUserGymClass AURemove = _context.ApplicationUserGymClass.Where(e => e.ApplicationUserId == user.Id && e.GymClassId == GC.Id).FirstOrDefault();

                _context.ApplicationUserGymClass.Remove(AURemove);
                user.AttendedClasses.Remove(AURemove);
                GC.AttendingMembers.Remove(AURemove);
            }
            else
            {
                ApplicationUserGymClass AGC = new ApplicationUserGymClass
                {
                    GymClassId = GC.Id,
                    GymClass = GC,
                    ApplicationUserId = user.Id,
                    ApplicationUser = user
                };
                user.AttendedClasses.Add(AGC);
                GC.AttendingMembers.Add(AGC);

                _context.ApplicationUserGymClass.Add(AGC);
            }

            GymClass GCTemp = _context.GymClasses.Where(e => e.Id == id).FirstOrDefault();
            _context.Entry(GCTemp).CurrentValues.SetValues(GC);

            ApplicationUser AUemp = _context.ApplicationUsers.Where(e => e.Id == user.Id).FirstOrDefault();
            _context.Entry(AUemp).CurrentValues.SetValues(user);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
