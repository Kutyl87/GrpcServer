using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Grpc.Core;
using LibraryGrpc.Models;
using System.Runtime.InteropServices;
using System.Reflection.Metadata.Ecma335;
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
            Surname = request.Surname
        };
        var result = await _userManager.CreateAsync(customer, request.Password);
        if (result.Succeeded)
            return new RegisterResponse
            {
                Message = "User created successfully",
                IsSuccess = true,
            };
        return new RegisterResponse
        {
            Message = "User was not created",
            IsSuccess = false,
        };

    }
    public override async Task<LogInResponse> LogInUser(LogInRequest request, ServerCallContext context)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if(user==null)
        {
            return new LogInResponse
            {
                Message = "There is no user associated with this password",
                IsSuccess = false
            };
        }
        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if(!result)
        {
            return new LogInResponse
                {
                    Message = "Invalid Password!",
                    IsSuccess = false
                };
        }
        return new LogInResponse
        {
            Message = "Succesfully logged in!",
            IsSuccess = true
        };
    }
}
