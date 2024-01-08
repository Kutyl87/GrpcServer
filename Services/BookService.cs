using System;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using LibraryGrpc.Data;
using LibraryGrpc.Models;
using System.Net;

namespace LibraryGrpc.Services;

public class BookService : BookIt.BookItBase
{
    private readonly DbContextClass _dbContext;

    public BookService(DbContextClass dbContext)
    {
        _dbContext = dbContext;
    }
    public override async Task<GetAllBookResponse> ListBook(GetAllBookRequest request, ServerCallContext context)
    {
        var response = new GetAllBookResponse();
        var Books = await _dbContext.Book.ToListAsync();

        foreach (var book in Books)
        {
            var readBookResponse = new ReadBookResponse
            {
                Id = book.BookId,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                Rating = book.Rating,
                Availability = book.Availability,
                Description = book.BookDescription
            };

            if (book.CurrentOwnerId != null)
            {
                readBookResponse.CurrentOwnerId = (int)book.CurrentOwnerId;
            }
            

            response.Book.Add(readBookResponse);
            

        }
        Console.WriteLine(response);
        return await Task.FromResult(response);
    }

    public override async Task<ReadBookResponse> ReadBook(ReadBookRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "resouce index must be greater than 0"));

        var book = await _dbContext.Book.FirstOrDefaultAsync(t => t.BookId == request.Id);

        if (book != null)
        {
            return await Task.FromResult(new ReadBookResponse
            {
                Id = book.BookId,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                Rating = book.Rating,
                Availability = book.Availability,
                Description = book.BookDescription


            });
        }
        throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
    }

    public override async Task<UpdateBookResponse> UpdateBook(UpdateBookRequest request, ServerCallContext context)
    {

        if (request.Id <= 0 || request.CurrentOwnerId <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must suppply a valid object"));

        var book = await _dbContext.Book.FirstOrDefaultAsync(t => t.BookId == request.Id);

        if (!book.Availability)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Book is not available"));

        if (book == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with Id {request.Id}"));

        book.Availability = false;
        book.CurrentOwnerId = request.CurrentOwnerId;

        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new UpdateBookResponse
        {
            Id = book.BookId
        });
    }

}