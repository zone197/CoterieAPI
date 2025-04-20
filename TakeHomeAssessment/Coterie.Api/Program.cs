using FluentValidation.AspNetCore;
using FluentValidation;
using NLog.Extensions.Logging;
using NLog.Web;
using MiniRater.Models;
using MiniRater.Services;
using MiniRater.Interfaces;
using Coterie.Api.Models.MiniRater;
using Coterie.Api.ExceptionHelpers;


var builder = WebApplication.CreateBuilder(args);

// Configure NLog
builder.Logging.ClearProviders();
builder.Logging.AddNLog();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<MiniRaterRequestModel>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IRateCalculatorService, RateCalculatorService>();
builder.Services.Configure<RateConfig>(builder.Configuration.GetSection("RateConfig"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

//app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseExceptionHandler(appBuilder => appBuilder.UseMiddleware<ErrorHandlerMiddleware>());

app.UseAuthorization();

app.MapControllers();

app.Run();
