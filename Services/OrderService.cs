using System;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using LibraryGrpc.Data;
using LibraryGrpc.Models;
using System.Net;
using Google.Protobuf.WellKnownTypes;

namespace LibraryGrpc.Services;

public class OrderService : OrderIt.OrderItBase
{
    private readonly DbContextClass _dbContext;

    public OrderService(DbContextClass dbContext)
    {
        _dbContext = dbContext;
    }
    public override async Task<GetAllOrderResponse> ListOrder(GetAllOrderRequest request, ServerCallContext context)
    {
        var response = new GetAllOrderResponse();
        var Orders = await _dbContext.Orders.ToListAsync();
        


        foreach (var order in Orders)
        {
            var UtcOrderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);
            var readOrderResponse = new ReadOrderResponse
            {
                Id = order.OrderId,
                OrderDate = Timestamp.FromDateTime(UtcOrderDate),
                BookId = order.BookId,
                CustomerId = order.CustomerId,
                State = order.State

            };
            if (order.ReturnDate != null)
            {
                var UtcReturnDate = DateTime.SpecifyKind((DateTime)order.ReturnDate, DateTimeKind.Utc);
                readOrderResponse.ReturnDate = Timestamp.FromDateTime(UtcReturnDate);
            }

            response.Order.Add(readOrderResponse);


        }
        Console.WriteLine(response);
        return await Task.FromResult(response);
    }

    public override async Task<ReadOrderResponse> ReadOrder(ReadOrderRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "resouce index must be greater than 0"));

        var order = await _dbContext.Orders.FirstOrDefaultAsync(t => t.OrderId == request.Id);
        

        if (order != null)
        {
            var UtcOrderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);
            var response = new ReadOrderResponse
            {
                Id = order.OrderId,
                OrderDate = Timestamp.FromDateTime(UtcOrderDate),
                BookId = order.BookId,
                CustomerId = order.CustomerId,
                State = order.State
            };

            if (order.ReturnDate != null)
            {
                var UtcReturnDate = DateTime.SpecifyKind((DateTime)order.ReturnDate, DateTimeKind.Utc);
                response.ReturnDate = Timestamp.FromDateTime(UtcReturnDate);
            }

            return await Task.FromResult(response);
        }
        throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
    }


}