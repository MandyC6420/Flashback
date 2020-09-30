using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flashback.Models
{
    public class Meeting
    {
        [Key]
        public int MeetingId { get; set; }
        public string UserId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string VenueChannel { get; set; }
        public string Agenda { get; set; }

        public ApplicationUser User { get; set; }

        public List<Message> Messages { get; set; }

        public List<Attendants> Attendants { get; set; }
    }
}
