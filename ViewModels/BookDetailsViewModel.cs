using bookspace.Models;

namespace bookspace.ViewModels
{
    public class BookDetailsViewModel
    {
        public Book Book { get; set; } = null!;
        public List<Discussion> Discussions { get; set; } = new();
        public int TotalDiscussions { get; set; }
        public int TotalComments { get; set; }
        public bool IsUserLoggedIn { get; set; }
    }
}
