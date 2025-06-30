using bookspace.Models;

namespace bookspace.ViewModels
{
    public class HomeViewModel
    {
        public List<Book> RecentBooks { get; set; } = new List<Book>();
        public List<Category> PopularCategories { get; set; } = new List<Category>();
    }
} 