using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ToDoListWebApp.Contexts;
using ToDoListWebApp.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using ToDoListWebApp.Models;

namespace ToDoListWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly UserManager<User> _userManager;

        public TodoItemsController(TodoContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("items/{itemId:long}")]
        public async Task<ActionResult<TodoItem>> GetItem(long itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var item = await _context.TodoItems
                .FirstOrDefaultAsync(i => i.UserId == user.Id && i.Id == itemId);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }


        [HttpGet("items")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetItems()
        {
 
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var items = await _context.TodoItems
                .Where(item => item.UserId == user.Id)
                .ToListAsync();

            return Ok(items);
        }


        [HttpPut("items/{itemId:long}")]
        public async Task<IActionResult> PutItem([FromRoute] long itemId, [FromBody] TodoItem item)
        {
     
            if (itemId != item.Id)
            {
                return NotFound("Item ID does not match the provided data.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // duplicity test
            var existingItem = await _context.TodoItems
                .SingleOrDefaultAsync(r => r.Id == itemId);

            if (existingItem == null)
            {
                return NotFound("Item not found.");
            }

         
            // Aktualizace existující položky
            existingItem.Title = item.Title;
            existingItem.Content = item.Content;
            existingItem.IsDone = item.IsDone;
            existingItem.Color = item.Color;

            await _context.SaveChangesAsync();

            return Ok(existingItem);
        }


        [HttpPost("items")]
        public async Task<ActionResult<TodoItem>> PostItem([FromBody] TodoItemModel item)
        {
 
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // duplicity test
            var duplicateItem = await _context.TodoItems
                .FirstOrDefaultAsync(r => r.Title == item.Title);

            if (duplicateItem != null)
            {
                return Conflict("Item with the same title already exists.");
            }

            var todoItemDAO = new TodoItem()
            {
                Title = item.Title,
                Content = item.Content,
                Color = item.Color,
                Created = DateTime.Now,  //This isn't a security hazard, it can be set on the frontend
                UserId = user.Id
            };

            _context.TodoItems.Add(todoItemDAO);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { itemId = todoItemDAO.Id });
        }

        [HttpDelete("items/{itemId:long}")]
        public async Task<IActionResult> DeleteItem([FromRoute] long itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var item = await _context.TodoItems
                .FirstOrDefaultAsync(i => i.UserId == user.Id && i.Id == itemId);

            if (item == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
