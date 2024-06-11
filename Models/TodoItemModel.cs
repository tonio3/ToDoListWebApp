using System.ComponentModel.DataAnnotations;

namespace ToDoListWebApp.Models;

public class TodoItemModel
{

    [Required]
    public string Title { get; set; }

    [MaxLength(300)]
    public string Content { get; set; }

    public bool IsDone { get; set; }

    
    public string Color { get; set; }

    [Required]
    public DateTime Created { get; set; }
 
}


