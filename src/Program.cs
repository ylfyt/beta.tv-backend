global using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(Constants.connectionString);
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

app.Run();
