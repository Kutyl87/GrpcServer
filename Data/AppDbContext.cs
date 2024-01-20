using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LibraryGrpc.Models;

namespace LibraryGrpc.Data;

public class DbContextClass : IdentityDbContext
{
    protected readonly IConfiguration Configuration;

    public DbContextClass(DbContextOptions options,IConfiguration configuration) : base(options)
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