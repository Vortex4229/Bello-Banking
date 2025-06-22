using Bello_Banking_API;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

// TODO: Figure out how to input values into an API call.
// TODO: Build controllers for each repository method.


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

string connectionString = builder.Configuration.GetConnectionString("belloBankingDB") ??
                          throw new Exception("No connection string found");
var conn = new MySqlConnection(connectionString);

app.MapGet("/getAccountBalance", () => {
        
    })
    .WithName("GetAccountBalance");

app.Run();
