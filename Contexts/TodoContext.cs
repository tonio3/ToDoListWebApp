using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoListWebApp.Entities;

namespace ToDoListWebApp.Contexts;

public class TodoContext : IdentityDbContext<User>
{

    public DbSet<TodoItem> TodoItems { get; set; }

    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
        ApplyMigrations(this);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=TodoItems.db;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>().ToTable("TodoItems");
      
        modelBuilder.Entity<TodoItem>()
        .HasIndex(p => p.Title)
        .IsUnique();
 
    }


    public void ApplyMigrations(TodoContext context)
    {
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
}