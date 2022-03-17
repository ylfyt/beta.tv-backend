global using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Interfaces;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("===============================");
var connectionString = Environment.GetEnvironmentVariable("connection-string");
if (connectionString == null)
{
    Console.WriteLine("Connection string in environment is not found. Get from constant");
    connectionString = Constants.connectionString;
}
else
{
    Console.WriteLine("Get onnection string from environment");
}
Console.WriteLine(connectionString);
Console.WriteLine("===============================");

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// TODO: Pilih yang terbaik
builder.Services.AddScoped<ITokenManager, TokenManager>();
// builder.Services.AddTransient<ITokenManager, TokenManager>();
// builder.Services.AddSingleton<ITokenManager, TokenManager>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.Run();
