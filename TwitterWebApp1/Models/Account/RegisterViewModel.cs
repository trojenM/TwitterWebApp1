using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TwitterWebApp1.Models.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Remote("IsUserNameInUse", "Account", ErrorMessage = "Username is already in use.")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [Remote("IsEmailInUse", "Account", ErrorMessage = "Email is already in use.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Name { get; set; }
    }
}