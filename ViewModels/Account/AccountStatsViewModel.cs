namespace bookspace.ViewModels.Account
{
    public class AccountStatsViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public DateTime MemberSince { get; set; }
        public DateTime? LastLogin { get; set; }
        public int BooksRead { get; set; }
        public int ReviewsWritten { get; set; }
        public int DiscussionsStarted { get; set; }
        public int TotalComments { get; set; }
        public int TotalDiscussions { get; set; }
    }
} 