using System.ComponentModel.DataAnnotations;

namespace bookspace.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Beni hatırla?")]
        public bool RememberMe { get; set; }
    }
} 