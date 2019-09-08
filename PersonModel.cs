using Microsoft.EntityFrameworkCore;
using System;

public class TodoEf
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public string TaskDescription { get; set; }
    public bool IsCompleted { get; set; }
}

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    { }

    public DbSet<TodoEf> Todos { get; set; }
}