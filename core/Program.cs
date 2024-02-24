using Microsoft.EntityFrameworkCore;
using NLog.Web;
using todo.Data;
using todo.Data.Repositories;
using todo.Exceptions;
using todo.Services;
using todo.Utils;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationContext")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.SetMinimumLevel(LogLevel.Information);
    loggingBuilder.AddNLog("nlog.config");
});

var app = builder.Build();

app.UsePathBase("/api/v1");
app.UseMiddleware<GlobalApiPrefixMiddleware>("/api/v1");

app.UseMiddleware<CustomErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();

public partial class Program { }