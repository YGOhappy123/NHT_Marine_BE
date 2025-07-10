using System.ComponentModel.DataAnnotations;

namespace NHT_Marine_BE.Models.User
{
    public class Conversation
    {
        [Key]
        public int ConversationId { get; set; }
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public List<ChatMessage> Messages { get; set; } = [];
    }
}
