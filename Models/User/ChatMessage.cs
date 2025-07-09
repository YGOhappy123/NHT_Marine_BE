using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NHT_Marine_BE.Models.User
{
    public class ChatMessage
    {
        [Key]
        public int MessageId { get; set; }
        public int? ConversationId { get; set; }
        public int? SenderStaffId { get; set; }
        public string TextContent { get; set; } = string.Empty;
        public string ImageContent { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.Now;
        public Conversation? Conversation { get; set; }
        public Staff? SenderStaff { get; set; }
    }
}
