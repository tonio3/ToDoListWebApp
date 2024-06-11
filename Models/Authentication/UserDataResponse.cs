using System.ComponentModel.DataAnnotations;

namespace ToDoListWebApp.Models.Authentication;

public class UserDataResponse
{
    public string Username { get; set; } = null!;

    [EmailAddress]
    public string Email { get; set; } = null!;

    public DateTime LatestLoginTime { get; set; }

    public int CreatedTodoItemsCount { get; set; }

    public int DeletedTodoItemsCount { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }
}
