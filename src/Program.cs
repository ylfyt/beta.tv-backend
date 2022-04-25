global using Microsoft.EntityFrameworkCore;
global using src.utils;
using src.Data;
using src.Dtos.category;
using src.Dtos.comment;
using src.Dtos.commentLike;
using src.Dtos.video;
using src.Interfaces;
using src.Services;

// Loading .env file
var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECT");
var betaPassword = Environment.GetEnvironmentVariable("BETA_EMAIL_PASSWORD");
var betaEmail = Environment.GetEnvironmentVariable("BETA_EMAIL_ADDRESS");
var adminPageUrl = Environment.GetEnvironmentVariable("ADMIN_PAGE_URL");
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");

if (connectionString == null || betaPassword == null || betaEmail == null || adminPageUrl == null || jwtSecret == null){
  throw new Exception("Some Environment Variabel is not exist");
}

Console.WriteLine("===============================");
Console.WriteLine($"DB_CONNECT={connectionString}");
Console.WriteLine($"BETA_EMAIL_ADDRESS={betaEmail}");
Console.WriteLine($"BETA_EMAIL_PASSWORD={betaPassword}");
Console.WriteLine($"ADMIN_PAGE_URL={adminPageUrl}");
Console.WriteLine($"JWT_SECRET={jwtSecret}");
Console.WriteLine("===============================");

ServerInfo.EMAIL_ADDRESS = betaEmail;
ServerInfo.EMAIL_PASSWORD = betaPassword;
ServerInfo.ADMIN_PAGE_URL = adminPageUrl;
ServerInfo.JWT_SECRET = jwtSecret;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(connectionString!);
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
