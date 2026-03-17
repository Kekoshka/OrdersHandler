using FluentValidation;
using PaymentService.DataAccess.Postgres.Context;
using PaymentService.WebApi.Common.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.RegisterMappers();
builder.Services.UsePostgreSql(builder.Configuration);
builder.Services.UseSeeding(builder.Configuration);
builder.Services.RegisterExecutingAsseblyServices();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.AddMediatR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandling();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
