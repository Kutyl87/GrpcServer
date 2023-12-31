using Microsoft.EntityFrameworkCore;
using LibraryGrpc.Models;

namespace LibraryGrpc.Data;

public class DbContextClass : DbContext
{
    protected readonly IConfiguration Configuration;

    public DbContextClass(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Configuration.GetConnectionString("AppDbConnectionString");
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version()));
    }

    public DbSet<Book> Book { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customer { get; set; }
}