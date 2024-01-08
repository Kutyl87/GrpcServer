using System;
using Grpc.Core;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LibraryGrpc.Data;
using LibraryGrpc.Models;
using System.Net;
using Google.Protobuf.WellKnownTypes;

namespace LibraryGrpc.Services;

public class StatisticsService : StatisticsIt.StatisticsItBase
{
    private readonly DbContextClass _dbContext;

    public StatisticsService(DbContextClass dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<GetBookHistoryResponse> ReadBookHistory(GetBookHistoryRequest request, ServerCallContext context)
    {
        Console.WriteLine("DDDDDD");
        var response = new GetBookHistoryResponse();
        var Orders = await _dbContext.Orders.ToListAsync();
        var Books = await _dbContext.Book.ToListAsync();
        var query = from order in Orders
                    join book in Books on order.OrderId equals book.BookId
                    select new
                    {
                        OrderId = order.OrderId,
                        OrderDate = order.OrderDate,
                        BookId = book.BookId,
                        CustomerId = order.CustomerId,
                        State = order.State,
                        BookTitle = book.Title,
                        BookAuthor = book.Author,
                        BookGenre = book.Genre,
                        ReturnDate = order.ReturnDate
                    };


        foreach (var elem in query)
        {
            var UtcOrderDate = DateTime.SpecifyKind(elem.OrderDate, DateTimeKind.Utc);
            var readBookHistoryResponse = new ReadBookHistoryResponse
            {
                Id = elem.OrderId,
                OrderDate = Timestamp.FromDateTime(UtcOrderDate),
                BookId = elem.BookId,
                CustomerId = elem.CustomerId,
                State = elem.State,
                BookTitle = elem.BookTitle,
                BookAuthor = elem.BookAuthor,
                BookGenre = elem.BookGenre,

            };
            if (elem.ReturnDate != null)
            {
                var UtcReturnDate = DateTime.SpecifyKind((DateTime)elem.ReturnDate, DateTimeKind.Utc);
                readBookHistoryResponse.ReturnDate = Timestamp.FromDateTime(UtcReturnDate);
            }

            response.Order.Add(readBookHistoryResponse);


        }
        Console.WriteLine(response);
        return await Task.FromResult(response);
    }

}