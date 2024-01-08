using System.ComponentModel.DataAnnotations;

namespace LibraryGrpc.Models;

public class Order
{
    [Key]
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int BookId { get; set; }

    public int CustomerId { get; set; }

    public string State { get; set; } = null!;

    public DateTime? ReturnDate { get; set; }



}