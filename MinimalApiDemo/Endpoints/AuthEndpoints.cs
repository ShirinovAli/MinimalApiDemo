using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApiDemo.DTOs;
using MinimalApiDemo.Models;
using MinimalApiDemo.Repositories.Abstract;
using System.Net;

namespace MinimalApiDemo.Endpoints
{
    public static class AuthEndpoints
    {
        public static void ConfigureAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/api/login", Login).WithName("Login").Accepts<LoginRequestDto>("application/json")
                                            .Produces<ApiResponse>((int)HttpStatusCode.OK).Produces((int)HttpStatusCode.BadRequest);
            app.MapPost("/api/register", Register).WithName("Register").Accepts<RegistrationRequestDto>("application/json")
                                                  .Produces<ApiResponse>((int)HttpStatusCode.OK).Produces((int)HttpStatusCode.BadRequest);
        }

        private async static Task<IResult> Login(IAuthRepository _authRepository, [FromBody] LoginRequestDto model)
        {
            ApiResponse apiResponse = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            var loginResponse = await _authRepository.Login(model);
            if (loginResponse == null)
            {
                apiResponse.ErrorMessages.Add("Username or password is incorrect");
                return Results.BadRequest(apiResponse);
            }

            apiResponse.Result = loginResponse;
            apiResponse.IsSuccess = true;
            apiResponse.StatusCode = HttpStatusCode.OK;

            return Results.Ok(apiResponse);
        }

        private async static Task<IResult> Register(IAuthRepository _authRepository, [FromBody] RegistrationRequestDto model)
        {
            ApiResponse apiResponse = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            bool ifUserNameIsUnique = _authRepository.IsUniqueUser(model.Username);
            if (!ifUserNameIsUnique)
            {
                apiResponse.ErrorMessages.Add("Username already exists");
                return Results.BadRequest(apiResponse);
            }

            var registerResponse = await _authRepository.Register(model);
            if (registerResponse == null || string.IsNullOrEmpty(registerResponse.Username))
            {
                return Results.BadRequest(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.StatusCode = HttpStatusCode.OK;
            return Results.Ok(apiResponse);
        }
    }
}
