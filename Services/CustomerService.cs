using System;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using LibraryGrpc.Data;
using LibraryGrpc.Models;
using System.Net;
using Google.Protobuf.WellKnownTypes;

namespace LibraryGrpc.Services;

public class CustomerService : CustomerIt.CustomerItBase
{
    private readonly DbContextClass _dbContext;

    public CustomerService(DbContextClass dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<GetAllCustomerResponse> ListCustomer(GetAllCustomerRequest request, ServerCallContext context)
    {
        var response = new GetAllCustomerResponse();
        var Customers = await _dbContext.Customer.ToListAsync();

        foreach (var customer in Customers)
        {
            var readCustomerResponse = new ReadCustomerResponse
            {
                Id = customer.CustomerId,
                Name = customer.Name,
                Surname = customer.Surname,
                Login = customer.Login,
                Mail = customer.Mail,
                Password = customer.Password

            };


            response.Customer.Add(readCustomerResponse);


        }
        Console.WriteLine(response);
        return await Task.FromResult(response);
    }

    public override async Task<ReadCustomerResponse> ReadCustomer(ReadCustomerRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "resouce index must be greater than 0"));

        var customer = await _dbContext.Customer.FirstOrDefaultAsync(t => t.CustomerId == request.Id);

        if (customer != null)
        {
            return await Task.FromResult(new ReadCustomerResponse
            {
                Id = customer.CustomerId,
                Name = customer.Name,
                Surname = customer.Surname,
                Login = customer.Login,
                Mail = customer.Mail,
                Password = customer.Password
   


            });
        }
        throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
    }
}