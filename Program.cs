using CampusPlacementManagement.Data;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories;
using CampusPlacementManagement.Repositories.Interfaces;
using CampusPlacementManagement.Services;
using CampusPlacementManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


//Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);




builder.Services.AddScoped<ICollegeRepository, CollegeRepository>();
builder.Services.AddScoped<
    IPlacementOfficerRepository,
    PlacementOfficerRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();


builder.Services.AddScoped<ICollegeService, CollegeService>();
builder.Services.AddScoped<
    IPlacementOfficerService,
    PlacementOfficerService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
