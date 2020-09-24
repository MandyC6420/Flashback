using Flashback.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Flashback.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<Meeting> Meeting { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<MeetingAttendant> MeetingAttendant { get; set; }


    }
}
