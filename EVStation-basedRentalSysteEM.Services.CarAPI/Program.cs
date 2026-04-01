using System.Text;
using EVStation_basedRentalSystem.Services.CarAPI.Data;
using EVStation_basedRentalSystem.Services.CarAPI.Services;
using EVStation_basedRentalSystem.Services.CarAPI.Services.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---------------- AutoMapper ----------------
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ---------------- DbContext ----------------
builder.Services.AddDbContext<CarDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------------- Controllers ----------------
builder.Services.AddControllers();

// ---------------- Swagger ----------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------- HTTP Clients for microservices ----------------
// 🔌 StationAPI client
builder.Services.AddHttpClient<IStationService, StationService>(c =>
{
    c.BaseAddress = new Uri("https://localhost:7004"); // StationAPI base URL
});

// 🔌 BookingAPI client
builder.Services.AddHttpClient<IBookingService, BookingService>(c =>
{
    c.BaseAddress = new Uri("https://localhost:7226"); // BookingAPI base URL
});

// ---------------- Scoped Services ----------------
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
            ValidAudience = builder.Configuration["JwtOptions:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:Secret"])
            )
        };
    });

var app = builder.Build();

// ---------------- Middleware ----------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
