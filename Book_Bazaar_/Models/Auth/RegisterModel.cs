using System.ComponentModel.DataAnnotations;

namespace Book_Bazaar_.Models.Auth
{
    public class RegisterModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        
    }
}
