using Microsoft.EntityFrameworkCore;
using todogrpcservice.Models;

namespace todogrpcservice.Data;
public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {

    }
    public DbSet<TodoItems> ToDoItems => Set<TodoItems>();  

}

