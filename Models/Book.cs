using System.ComponentModel.DataAnnotations;

namespace LibraryGrpc.Models;

public class Book
{
    [Key]
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string? Genre { get; set; }

    public float Rating { get; set; }

    public bool Availability { get; set; }

    public string BookDescription { get; set; } = null!;

    public int? CurrentOwnerId { get; set; }

    public string ImageUrl { get; set; } = null!;



}