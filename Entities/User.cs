using Microsoft.AspNetCore.Identity;

namespace ToDoListWebApp.Entities
{
    public class User : IdentityUser
    {

        public DateTime LatestLoginTime { get; set; }
        public int CreatedTodoItemsCount { get; set; }
        public int DeletedTodoItemsCount { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}