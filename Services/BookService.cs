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

    public override async Task<GetBooksByCategoryResponse> GetBooksByCategory(GetBooksByCategoryRequest request, ServerCallContext context)
    {
        Console.WriteLine("testy");
        var response = new GetBooksByCategoryResponse();
        var Books = await _dbContext.Book.ToListAsync();

        foreach (var book in Books)
        {
            if (book.Genre == request.Category)
            {
                var readBookResponse = new ReadBookResponse
                {
                    Id = book.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    Genre = book.Genre,
                    Rating = book.Rating,
                    Availability = book.Availability,
                    Description = book.BookDescription,
                    ImageUrl = book.ImageUrl
                };

                if (book.CurrentOwnerId != null)
                {
                    readBookResponse.CurrentOwnerId = (int)book.CurrentOwnerId;
                }


                response.Book.Add(readBookResponse);
            }
        }
        Console.WriteLine(response);
        return await Task.FromResult(response);
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
                Description = book.BookDescription,
                ImageUrl = book.ImageUrl
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

    public override async Task<ListCategoriesResponse> ListCategories(ListCategoriesRequest request, ServerCallContext context)
    {

        var response = new ListCategoriesResponse();
        var Books = await _dbContext.Book.ToListAsync();
        List<string> uniqueCategories = Books.Select(book => book.Genre).Distinct().ToList();

        foreach (string category in uniqueCategories)
        {
            response.Category.Add(category);
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
                Description = book.BookDescription,
                ImageUrl = book.ImageUrl


            });
        }
        throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
    }

    public override async Task<GetBookByTitleResponse> GetBookByTitle(GetBookByTitleRequest request, ServerCallContext context)
    {
        /*if (request.Title == "")
            throw new RpcException(new Status(StatusCode.InvalidArgument, "resouce index must be greater than 0"));*/
        var response = new GetBookByTitleResponse();
        var Books = await _dbContext.Book.ToListAsync();

        foreach (var book in Books)
        {
            if (book.Title.ToLower().Contains(request.Title.ToLower()))
            {
                var readBookResponse = new ReadBookResponse
                {
                    Id = book.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    Genre = book.Genre,
                    Rating = book.Rating,
                    Availability = book.Availability,
                    Description = book.BookDescription,
                    ImageUrl = book.ImageUrl
                };

                if (book.CurrentOwnerId != null)
                {
                    readBookResponse.CurrentOwnerId = (int)book.CurrentOwnerId;
                }


                response.Book.Add(readBookResponse);
            }

        }
        Console.WriteLine(response);
        return await Task.FromResult(response);
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

    public override async Task<ReturnBookResponse> ReturnBook(ReturnBookRequest request, ServerCallContext context)
    {


        Console.WriteLine(request.CurrentOwnerId);
        Console.WriteLine(request.Id);
        Console.WriteLine(request.Id <= 0);
        Console.WriteLine(request.CurrentOwnerId <= 0);


        if (request.Id <= 0 || request.CurrentOwnerId <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must suppply a valid object"));

        var book = await _dbContext.Book.FirstOrDefaultAsync(t => t.BookId == request.Id && t.CurrentOwnerId == request.CurrentOwnerId);

        if (book == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with Id {request.Id}"));

        book.Availability = true;
        book.CurrentOwnerId = null;

        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new ReturnBookResponse
        {
            Id = book.BookId
        });
    }
    public override async Task<ListAllBooksByCustomerIdResponse> GetBooksByUserId(ListAllBooksByCustomerIdRequest request, ServerCallContext context)
    {
        var response = new ListAllBooksByCustomerIdResponse();
        var books = await _dbContext.Book.Where(t => t.CurrentOwnerId == request.UserId).ToListAsync();
        if (books == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"No User with Id {request.UserId}"));
        foreach (var book in books)
        {
            var readBookResponse = new ReadBookResponse
            {
                Id = book.BookId,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                Rating = book.Rating,
                Availability = book.Availability,
                Description = book.BookDescription,
                ImageUrl = book.ImageUrl
            };

            if (book.CurrentOwnerId != null)
            {
                readBookResponse.CurrentOwnerId = (int)book.CurrentOwnerId;
            }
            response.Book.Add(readBookResponse);
        }

        return await Task.FromResult(response);

    }

}