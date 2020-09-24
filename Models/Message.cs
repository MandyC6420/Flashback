using System;
using System.ComponentModel.DataAnnotations;

namespace Flashback.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Messages { get; set; }
        public int MeetingId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public Meeting Meeting { get; set; }

    }
}
