using Microsoft.AspNetCore.Identity;

namespace bookspace.Models
{
    public class User : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Profile Information
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? Website { get; set; }
        public string? ProfilePicture { get; set; }
        
        // Account Statistics
        public int BooksRead { get; set; } = 0;
        public int ReviewsWritten { get; set; } = 0;
        public int DiscussionsStarted { get; set; } = 0;
        public DateTime? LastLoginAt { get; set; }
        
        // Navigation Properties
        public virtual ICollection<Book> FavoriteBooks { get; set; } = new List<Book>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();
        
        // Computed Properties
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string DisplayName => !string.IsNullOrEmpty(FullName) ? FullName : UserName ?? Email ?? "Unknown User";
    }
}
