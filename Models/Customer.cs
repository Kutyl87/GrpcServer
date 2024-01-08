using System.ComponentModel.DataAnnotations;

namespace LibraryGrpc.Models;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Mail { get; set; } = null!;

    public string Password { get; set; } = null!;


}