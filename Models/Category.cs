using System.ComponentModel.DataAnnotations;

namespace LibraryGrpc.Models;

public class Category
{
    [Key]
    public string CategoryName { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;


}