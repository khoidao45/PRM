using EVStation_basedRentalSystem.Services.PaymentAPI.Data;
using EVStation_basedRendtalSystem.Services.PaymentAPI.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentDbConnection")));

// Booking API
builder.Services.AddHttpClient<IBookingService, BookingService>();



// PayOSService
builder.Services.AddSingleton<PayOSService>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
