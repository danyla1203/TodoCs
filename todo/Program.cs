﻿using Microsoft.EntityFrameworkCore;
using todo.Data;
using todo.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<UnitOfWork>();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationContext")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

