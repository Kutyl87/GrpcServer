using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Grpc.Core;
using LibraryGrpc.Models;
using System.Runtime.InteropServices;
namespace LibraryGrpc.Services;

public class AuthService : AuthIt.AuthItBase
{
    private readonly UserManager<Customer> _userManager;
    public AuthService(UserManager<Customer> userManager)
    {
        _userManager = userManager;
    }
    public override async Task<RegisterResponse> RegisterUser(RegisterRequest request, ServerCallContext context)
    {
        if (request == null)
        {
            throw new NullReferenceException("Register Model is Null");
        };
        if (request.Password != request.ConfirmPassword)
            return new RegisterResponse
            {
                Message = "Passwords are not equal",
                IsSuccess = false
            };
        var customer = new Customer
        {
            Email = request.Email,
            UserName = request.Email,
            Name = request.Name,
            Surname = request.Surname,
            Login = request.Email
        };
        var result = await _userManager.CreateAsync(customer, request.Password);
        Console.WriteLine(result);
        if (result.Succeeded)
            return new RegisterResponse
            {
                Message = "User created successfully",
                IsSuccess = true,
            };
        return new RegisterResponse
        {
            Message = "User did not create",
            IsSuccess = false,
        };
    }
}
