using Flashback.Data;
using Flashback.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Flashback.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;


        public MessagesController(ApplicationDbContext ctx,
                          UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Message.Include(m => m.Meeting).Include(m => m.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.Meeting)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create(int? id)
        {
            ViewData["MeetingId"] = new SelectList(_context.Meeting, "MeetingId", "MeetingId");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,UserId,TimeStamp,Messages,MeetingId")] Message message, int meetingid, DateTime timestamp)
        {
            var user = await GetCurrentUserAsync();
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {

                message.UserId = user.Id;
                message.MeetingId = meetingid;
                message.TimeStamp = DateTime.Now;
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Meetings", new { id = meetingid });
            }
            ViewData["MeetingId"] = new SelectList(_context.Meeting, "MeetingId", "MeetingId", message.MeetingId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", message.UserId);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["MeetingId"] = new SelectList(_context.Meeting, "MeetingId", "MeetingId", message.MeetingId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", message.UserId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,UserId,TimeStamp,Messages,MeetingId")] Message message)
        {
            if (id != message.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageId))
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
            ViewData["MeetingId"] = new SelectList(_context.Meeting, "MeetingId", "MeetingId", message.MeetingId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", message.UserId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.Meeting)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Message.FindAsync(id);
            _context.Message.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Message.Any(e => e.MessageId == id);
        }
    }
}
