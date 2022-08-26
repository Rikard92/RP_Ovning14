using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RP_Övning14.Core.Entities;
using RP_Övning14.Data;
using RP_Övning14.ViewModels;

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
            var userID = _userManager.GetUserId(User);
            var viewModel = await GetGymClassesForUser(userID);
              return View(viewModel);
        }

        public async Task<IActionResult> Bokade()
        {
            var userID = _userManager.GetUserId(User);
            var viewModel = await GetGymBokedClassesForUser(userID);
            return View(viewModel);
        }

        private async Task<List<GymClassesViewModel>> GetGymBokedClassesForUser(string userID)
        {
            return await _context.GymClasses.Include(l => l.AttendingMembers).Select(x => new GymClassesViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Duration = x.Duration,
                StartTime = x.StartTime,
                isUserAttending = x.AttendingMembers.Any(x => x.ApplicationUserId == userID)

            }).Where(x => x.isUserAttending == true).ToListAsync();
        }

        private async Task<List<GymClassesViewModel>> GetGymClassesForUser(string userID)
        {
            return await _context.GymClasses.Include(l => l.AttendingMembers).Select(x => new GymClassesViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Duration = x.Duration,
                StartTime = x.StartTime,                
                isUserAttending = x.AttendingMembers.Any(x=> x.ApplicationUserId == userID)

            }).ToListAsync();
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
            if (!ModelState.IsValid)
            {
                gymClass.Duration = gymClass.Duration / 24;
                gymClass.AttendingMembers = new List<ApplicationUserGymClass>();
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
            if(id == null) return BadRequest();        

            //ApplicationUser user = await _userManager.GetUserAsync(User);


            var userID = _userManager.GetUserId(User);
            if (userID == null) return BadRequest();

            //GymClass GC = _context.GymClasses.Include(l => l.AttendingMembers).FirstOrDefault(e => e.Id == id);
            //if (GC == null) return BadRequest();

            //var attending = GC?.AttendingMembers.FirstOrDefault(e => e.ApplicationUserId == userID);

            var attending = await _context.ApplicationUserGymClass.FindAsync(userID, id);
            if (attending == null)
            {
                ApplicationUserGymClass AGC = new ApplicationUserGymClass
                {
                    GymClassId = (int)id,
                    ApplicationUserId = userID,
                };
                _context.ApplicationUserGymClass.Add(AGC);
            }
            else
            {
                _context.ApplicationUserGymClass.Remove(attending);
            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
