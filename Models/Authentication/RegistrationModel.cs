using System.ComponentModel.DataAnnotations;

namespace ToDoListWebApp.Models.Authentication
{
    public class RegistrationModel
    {
        public required string Username { get; set; }

        public required string Password { get; set; }

        [EmailAddress]
        public required string Email { get; set; }
    }
}