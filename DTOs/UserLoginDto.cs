     // DTOs/UserRegisterDto.cs
     using System.ComponentModel.DataAnnotations;

     namespace workOut.DTOs
     {
         public class UserRegisterDto
         {
             [Required]
             [EmailAddress]
             public string Email { get; set; }

             [Required]
             [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
             public string Password { get; set; }
         }
     }