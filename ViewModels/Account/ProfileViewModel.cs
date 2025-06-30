using System.ComponentModel.DataAnnotations;

namespace bookspace.ViewModels.Account
{
    public class ProfileViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "First name must be between {2} and {1} characters.", MinimumLength = 2)]
        public string? FirstName { get; set; }
        
        [StringLength(50, ErrorMessage = "Last name must be between {2} and {1} characters.", MinimumLength = 2)]
        public string? LastName { get; set; }

        [StringLength(500, ErrorMessage = "Bio must not exceed {1} characters.")]
        public string? Bio { get; set; }

        [StringLength(100, ErrorMessage = "Location must not exceed {1} characters.")]
        public string? Location { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? Website { get; set; }
    }
} 