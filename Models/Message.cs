using System;
using System.ComponentModel.DataAnnotations;

namespace Flashback.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public string UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Messages { get; set; }
        public int MeetingId { get; set; }

        public ApplicationUser User { get; set; }

        public Meeting Meeting { get; set; }

    }
}
