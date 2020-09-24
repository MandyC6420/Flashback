using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flashback.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {

        }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public List<Meeting> Meetings { get; set; }
        public List<MeetingAttendant> MeetingAttendants { get; set; }
        public List<Message> Messages { get; set; }
    }
}
