using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalApiDemo.DTOs;
using MinimalApiDemo.Models;
using MinimalApiDemo.Repositories.Abstract;
using System.Net;

namespace MinimalApiDemo.Endpoints
{
    public static class CouponEndpoints
    {
        public static async void ConfigureCouponEndpoints(this WebApplication app)
        {
            app.MapGet("/api/coupon", GetAllCoupons).WithName("GetCoupons")
                    .Produces<ApiResponse>((int)HttpStatusCode.OK).RequireAuthorization("AdminOnly"); ;

            app.MapGet("/api/coupon/{id:int}", GetCoupon).WithName("GetCoupon").Produces<ApiResponse>((int)HttpStatusCode.OK);

            app.MapPost("/api/coupon", CreateCoupon).WithName("CreateCoupon").Accepts<CouponCreateDto>("application/json").Produces<ApiResponse>((int)HttpStatusCode.Created).Produces((int)HttpStatusCode.BadRequest);

            app.MapPut("/api/coupon", UpdateCoupon).WithName("UpdateCoupon").Accepts<CouponUpdateDto>("application/json")
              .Produces<ApiResponse>((int)HttpStatusCode.OK).Produces((int)HttpStatusCode.BadRequest);

            app.MapDelete("/api/coupon/{id:int}", DeleteCoupon);
        }


        private async static Task<IResult> GetAllCoupons(ICouponRepository _couponRepository, ILogger<Program> _logger)
        {
            ApiResponse apiResponse = new();
            _logger.Log(LogLevel.Information, "Get all counpons");
            apiResponse.Result = await _couponRepository.GetAllAsync();
            apiResponse.IsSuccess = true;
            apiResponse.StatusCode = HttpStatusCode.OK;

            return Results.Ok(apiResponse);
        }

        private async static Task<IResult> DeleteCoupon(ICouponRepository _couponRepository, int id)
        {
            ApiResponse apiResponse = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            Coupon couponFromStore = await _couponRepository.GetAsync(id);
            if (couponFromStore == null)
            {
                apiResponse.ErrorMessages.Add("Data not found");
                return Results.BadRequest(apiResponse);
            }
            else
            {
                await _couponRepository.RemoveAsync(couponFromStore);
                await _couponRepository.SaveAsync();

                apiResponse.IsSuccess = true;
                apiResponse.StatusCode = HttpStatusCode.NoContent;
                return Results.Ok(apiResponse);
            }
        }

        private async static Task<IResult> UpdateCoupon(ICouponRepository _couponRepository, IMapper _mapper, IValidator<CouponUpdateDto> _validator, [FromBody] CouponUpdateDto counpon_U_DTO)
        {
            ApiResponse apiResponse = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            var validationResult = await _validator.ValidateAsync(counpon_U_DTO);
            if (!validationResult.IsValid)
            {
                apiResponse.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                return Results.BadRequest(apiResponse);
            }

            Coupon couponFromStore = await _couponRepository.GetAsync(counpon_U_DTO.Id);
            if (couponFromStore == null)
            {
                apiResponse.ErrorMessages.Add("Data not found");
                return Results.NotFound(apiResponse);
            }

            await _couponRepository.UpdateAsync(_mapper.Map<Coupon>(counpon_U_DTO));
            await _couponRepository.SaveAsync();

            apiResponse.Result = _mapper.Map<CouponDto>(await _couponRepository.GetAsync(counpon_U_DTO.Id));
            apiResponse.IsSuccess = true;
            apiResponse.StatusCode = HttpStatusCode.OK;

            return Results.Ok(apiResponse);
        }

        private async static Task<IResult> CreateCoupon(ICouponRepository _couponRepository, IMapper _mapper, IValidator<CouponCreateDto> _validator, [FromBody] CouponCreateDto coupon_C_DTO)
        {
            ApiResponse apiResponse = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            var validationResult = await _validator.ValidateAsync(coupon_C_DTO);
            if (!validationResult.IsValid)
            {
                apiResponse.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                return Results.BadRequest(apiResponse);
            }

            if (await _couponRepository.GetAsync(coupon_C_DTO.Name) != null)
            {
                apiResponse.ErrorMessages.Add("Coupon Name already exists");
                return Results.BadRequest(apiResponse);
            }

            Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);
            await _couponRepository.CreateAsync(coupon);
            await _couponRepository.SaveAsync();

            CouponDto couponDto = _mapper.Map<CouponDto>(coupon);

            apiResponse.Result = couponDto;
            apiResponse.IsSuccess = true;
            apiResponse.StatusCode = HttpStatusCode.Created;
            return Results.Ok(apiResponse);
        }

        private async static Task<IResult> GetCoupon(ICouponRepository _couponRepository, int id)
        {
            ApiResponse apiResponse = new();
            apiResponse.Result = await _couponRepository.GetAsync(id);
            apiResponse.IsSuccess = true;
            apiResponse.StatusCode = HttpStatusCode.OK;
            return Results.Ok(apiResponse);
        }
    }
}
