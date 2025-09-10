using BookFinder.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace BookFinder.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Author> Authors
    {
        get;
        set;
    }

    public DbSet<Book> Books
    {
        get;
        set;
    }

    public DbSet<User> Users
    {
        get;
        set;
    }
}