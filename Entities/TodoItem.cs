using System.ComponentModel.DataAnnotations;

namespace ToDoListWebApp.Entities
{
    public class TodoItem
    {
        [Required, Key]
        public long Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public string Title { get; set; }

        [MaxLength(300)]
        public string Content { get; set; }

        public string Color { get; set; }

        public bool IsDone { get; set; }
 
    }
}