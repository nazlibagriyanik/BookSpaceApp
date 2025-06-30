using bookspace.Models;

namespace bookspace.ViewModels
{
    public class DiscussionViewModel
    {
        public Discussion Discussion { get; set; } = null!;
        public List<Comment> Comments { get; set; } = new();
        public string NewCommentContent { get; set; } = string.Empty;
        public bool IsUserLoggedIn { get; set; }
        public bool CanEditDiscussion { get; set; }
        public bool CanDeleteDiscussion { get; set; }
    }
}
