using System;
using System.Collections.Generic;

namespace bookspace.Models
{
    public class Discussion
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; } = null!;
        public string UserId { get; set; } = string.Empty;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
