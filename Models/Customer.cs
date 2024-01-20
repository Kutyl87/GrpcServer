using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace LibraryGrpc.Models;

public class Customer : IdentityUser
{
    // [Key]
    // public int CustomerId { get; set; }

    public int NumberId { get; set; }
    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Login { get; set; } = null!;

    // public string Mail { get; set; } = null!;

    // public string Password { get; set; } = null!;


}