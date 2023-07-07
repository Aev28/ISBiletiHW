using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Domain.DTO
{
    public class UserRegistrationDto
    {
        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter your surname")]
        [StringLength(100)]
        public string Surname { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "Please enter an email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter a password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "The Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
