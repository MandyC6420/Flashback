﻿using Flashback.Data;
using Flashback.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flashback.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public MeetingsController(ApplicationDbContext ctx,
                          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Meetings
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? id, int? pageNumber)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            var user = await GetCurrentUserAsync();
            //trying to pull the user for my meetings display

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;



            IQueryable<Meeting> applicationDbContext;
            if (id == null)
            {
                applicationDbContext = _context.Meeting.Include(m => m.User);


                ViewData["createHTML"] = "Create New";
            }
            else
            {
                applicationDbContext = _context.Meeting.Include(m => m.User).Include(m => m.Attendants).Where(m => m.Attendants.Any(a => a.UserId == user.Id));
            }

            int pageSize = 3;



            return View(await PaginatedList<Meeting>.CreateAsync(applicationDbContext.AsNoTracking(), pageNumber ?? 1, pageSize));


            return View(applicationDbContext);

        }

        // GET: Meetings/Details/5
        public async Task<IActionResult> Details(int? id, List<Meeting> applicationDbContext)
        {
            var user = await GetCurrentUserAsync();

            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meeting
                .Include(m => m.Attendants)
                .Include(m => m.User)
                .Include(m => m.Messages)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            var UserInMeeting = await _context.MeetingAttendant
                            .Where(ma => ma.MeetingId == id)
                            .Where(ma => ma.UserId == user.Id)
                            .ToListAsync();

            if (UserInMeeting.Count() == 0)
            {
                ViewBag.isAttending = false;
            }
            else
            {
                ViewBag.isAttending = true;
            }

            return View(meeting);


        }



        // GET: Meetings/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MeetingId,UserId,StartDateTime,EndDateTime,VenueChannel,Agenda")] Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                meeting.UserId = user.Id;
                _context.Add(meeting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", meeting.UserId);
            return View(meeting);
        }

        public async Task<ActionResult> JoinMeeting([Bind("MeetingAttendantId, MeetingId, UserId")] Meeting meetings, int id)
        {
            if (ModelState.IsValid)
            {
                MeetingAttendant Attendees = new MeetingAttendant();

                //Get the current user
                var user = await GetCurrentUserAsync();
                Attendees.UserId = user.Id;
                Attendees.MeetingId = id;
                _context.Add(Attendees);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Join Successful";
                return RedirectToAction("Details", "Meetings", new { id });
            }

            return View();
            //}


            //instance of MeetingAttendant
            //MeetingAttendant Attendants = new MeetingAttendant();

            //{
            //    MeetingId = MeetingId,
            //    UserId = UserId
            //};
            //_context.Add(Attendants);
            //await _context.SaveChangesAsync();

            //ViewData["MeetingId"] = new SelectList(_context.Meeting, "MeetingId", "UserId", Attendants
            //.MeetingId);
            //return View(Attendants);
        }

        // GET: Meetings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meeting.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", meeting.UserId);
            return View(meeting);
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MeetingId,UserId,StartDateTime,EndDateTime,VenueChannel,Agenda")] Meeting meeting)
        {
            if (id != meeting.MeetingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meeting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeetingExists(meeting.MeetingId))
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
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", meeting.UserId);
            return View(meeting);
        }

        // GET: Meetings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meeting
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MeetingId == id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // POST: Meetings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await GetCurrentUserAsync();
            var meetingAttendant = await _context.MeetingAttendant.FirstOrDefaultAsync(ma => ma.MeetingId == id && ma.UserId == user.Id);
            _context.MeetingAttendant.Remove(meetingAttendant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeetingExists(int id)
        {
            return _context.Meeting.Any(e => e.MeetingId == id);
        }
    }
}
