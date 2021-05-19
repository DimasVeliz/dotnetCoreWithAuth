using System;
using System.ComponentModel.DataAnnotations;

namespace dotnetCoreWithJWTAuth.Models.DTO
{
    public class UserRegistrationDto
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get;  set; }

        [Required]
        public string Password { get;  set; }

        [Required]
        public int Role { get; set; }
    }
}