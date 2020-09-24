using System.ComponentModel.DataAnnotations;

namespace Flashback.Models
{
    public class MeetingAttendant
    {
        [Key]
        public int MeetingAttendantId { get; set; }
        public int MeetingId { get; set; }
        public string UserId { get; set; }

        public Meeting Meeting { get; set; }

        public ApplicationUser User { get; set; }

    }
}
