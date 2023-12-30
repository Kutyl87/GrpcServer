using System;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using LibraryGrpc.Data;
using LibraryGrpc.Models;
using System.Net;

namespace LibraryGrpc.Services;

public class BookService : BookIt.BookItBase {
    private readonly DbContextClass _dbContext;

    public BookService(DbContextClass dbContext)
    {
        _dbContext = dbContext;
    }
    public override async Task<GetAllResponse> ListBook(GetAllRequest request, ServerCallContext context)
    {
        var response = new GetAllResponse();
        var Books = await _dbContext.Books.ToListAsync();

        foreach (var book in Books)
        {
            response.Book.Add(new ReadBookResponse
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                Rating = book.Rating,
                Availability = book.Availability,
                CurrentOwnerId = book.CurrentOwnerId
            });
        }

        return await Task.FromResult(response);
    }
}