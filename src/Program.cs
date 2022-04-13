global using Microsoft.EntityFrameworkCore;
global using src.utils;
using src.Data;
using src.Dtos.category;
using src.Dtos.comment;
using src.Dtos.commentLike;
using src.Dtos.video;
using src.Interfaces;
using src.Services;

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
    Console.WriteLine("Get connection string from environment");
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
builder.Services.AddScoped<IEmailTokenManager, EmailTokenManager>();
// builder.Services.AddTransient<ITokenManager, TokenManager>();
// builder.Services.AddSingleton<ITokenManager, TokenManager>();
builder.Services.AddScoped<IResponseGetter<DataCategory>, ResponseGetter<DataCategory>>();
builder.Services.AddScoped<IResponseGetter<DataCategories>, ResponseGetter<DataCategories>>();
builder.Services.AddScoped<IResponseGetter<DataVideo>, ResponseGetter<DataVideo>>();
builder.Services.AddScoped<IResponseGetter<DataVideos>, ResponseGetter<DataVideos>>();
builder.Services.AddScoped<IResponseGetter<DataComment>, ResponseGetter<DataComment>>();
builder.Services.AddScoped<IResponseGetter<DataComments>, ResponseGetter<DataComments>>();
builder.Services.AddScoped<IResponseGetter<DataCommentLike>, ResponseGetter<DataCommentLike>>();
builder.Services.AddScoped<IResponseGetter<DataCommentLikes>, ResponseGetter<DataCommentLikes>>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corsapp");
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
