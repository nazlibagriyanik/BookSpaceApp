using System;

namespace bookspace.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int DiscussionId { get; set; }
        public virtual Discussion Discussion { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public virtual User User { get; set; } = null!;
    }
}
