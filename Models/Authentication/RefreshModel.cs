using System.ComponentModel.DataAnnotations;

namespace ToDoListWebApp.Models.Authentication
{
    public class RefreshModel
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
