using ExceptionHandler;
using FluentValidation;
using OrderService.DataAccess.Postgres.Context;
using OrderService.WebApi.Common.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.UsePostgreSql(builder.Configuration);
builder.Services.RegisterExecutingAsseblyServices();
builder.Services.RegisterMappers();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.ConfigureOptions(builder.Configuration);
builder.Services.AddMediatR();
builder.Services.AddRefit(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//app.UseExceptionHandling();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers();

app.Run();
