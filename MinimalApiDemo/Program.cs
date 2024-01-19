using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApiDemo.Data;
using MinimalApiDemo.DTOs;
using MinimalApiDemo.Endpoints;
using MinimalApiDemo.Mappings;
using MinimalApiDemo.Models;
using MinimalApiDemo.Repositories.Abstract;
using MinimalApiDemo.Repositories.Concrete;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ICouponRepository, CouponRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
                              option.UseSqlServer(builder.Configuration.GetConnectionString("MsSql")));
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var app = builder.Build();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.ConfigureCouponEndpoints();

app.Run();