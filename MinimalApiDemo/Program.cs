using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApiDemo.Data;
using MinimalApiDemo.DTOs;
using MinimalApiDemo.Endpoints;
using MinimalApiDemo.Mappings;
using MinimalApiDemo.Models;
using MinimalApiDemo.Repositories.Abstract;
using MinimalApiDemo.Repositories.Concrete;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
        "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
        "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddTransient<ICouponRepository, CouponRepository>();
builder.Services.AddTransient<IAuthRepository, AuthRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
                              option.UseSqlServer(builder.Configuration.GetConnectionString("MsSql")));
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
           Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("APISettings:Secret"))),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});
var app = builder.Build();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.ConfigureCouponEndpoints();
app.ConfigureAuthEndpoints();
app.UseHttpsRedirection();

app.Run();