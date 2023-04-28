using System.ComponentModel.DataAnnotations;

namespace Book_Bazaar_.Models.Tables
{
    public class Users
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [MaxLength(8)]
        public string Password { get; set; } = string.Empty;
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        public bool IsVendor { get; set; } = false;
    }
}
