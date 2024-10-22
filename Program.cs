using Microsoft.EntityFrameworkCore;
using TransactionAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Set the HTTPS Redirection Options (set the HTTPS port)
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 7039; // Explicitly set the HTTPS port
});

// Configure the DbContext with Npgsql and migrations history table
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "data")
    )
);

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Enforce HTTPS Redirection
app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// Enable Authorization
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();
