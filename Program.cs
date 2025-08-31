using Microsoft.EntityFrameworkCore;
using System;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.Models;

var builder = WebApplication.CreateBuilder(args);
//Register EventPolicy from appsettings.json
builder.Services.Configure<EventPolicy>(builder.Configuration.GetSection("EventPolicy"));


// Add services to the container.
builder.Services.AddDbContext<WeddingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
