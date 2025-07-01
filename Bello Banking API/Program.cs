using Bello_Banking_API;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// connection management
string connectionString = builder.Configuration.GetConnectionString("belloBankingDB") ??
                          throw new Exception("No connection string found");
var conn = new MySqlConnection(connectionString);

// root method API calls
app.MapGet("/api/register/{username}/{password}/{email}/{firstName}/{lastName}", 
    (string username, string password, string email, string firstName, string lastName) => 
        $"{RootMethodsRepo.Register(conn, username, password, email, firstName, lastName)}").WithName("Register");

app.MapGet("/api/login/{username}/{password}", (string username, string password) => 
    $"{RootMethodsRepo.Login(conn, username, password)}").WithName("Login");

// account management API calls
app.MapGet("/api/getName/{userId}", (ulong userId) => $"{AccountManagementRepo.GetName(conn, userId)}")
    .WithName("GetName");   

app.MapGet("/api/checkBalance/{userId}", (ulong userId) => $"{AccountManagementRepo.CheckBalance(conn, userId)}")
    .WithName("CheckBalance");

app.MapGet("/api/updateBalance/{userId}/{amount}/{type}", (ulong userId, long amount, byte type) =>
    $"{AccountManagementRepo.UpdateBalance(conn, userId, amount, type)}").WithName("UpdateBalance");

app.MapGet("/api/sendMoney/{username}/{amount}", (string username, long amount) =>
    $"{AccountManagementRepo.SendMoney(conn, username, amount)}").WithName("SendMoney");

app.MapGet("/api/changeUsername/{userId}/{newUsername}", (ulong userId, string newUsername) =>
    $"{AccountManagementRepo.ChangeUsername(conn, userId, newUsername)}").WithName("ChangeUsername");

app.MapGet("/api/changePassword/{userId}/{oldPassword}/{newPassword}",
    (ulong userId, string oldPassword, string newPassword) =>
        $"{AccountManagementRepo.ChangePassword(conn, userId, oldPassword, newPassword)}"
).WithName("ChangePassword");

app.MapGet("/api/changeName/{userId}/{newFirstName}/{newLastName}",
    (ulong userId, string newFirstName, string newLastName) =>
        $"{AccountManagementRepo.ChangeName(conn, userId, newFirstName, newLastName)}").WithName("ChangeName");

app.MapGet("/api/changeEmail/{userId}/{newEmail}", (ulong userId, string newEmail) =>
    $"{AccountManagementRepo.ChangeEmail(conn, userId, newEmail)}").WithName("ChangeEmail");

app.MapGet("/api/deleteAccount/{userId}/{password}", (ulong userId, string password) =>
    $"{AccountManagementRepo.DeleteAccount(conn, userId, password)}").WithName("DeleteAccount");

app.Run();

