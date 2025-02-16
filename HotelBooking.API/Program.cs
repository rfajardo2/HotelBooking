using HotelBooking.Application.Interfaces;
using HotelBooking.Application.Services;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Infrastructure;
using HotelBooking.Infrastructure.Data;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<EmailService>();

builder.Services.AddDbContext<HotelBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelService, HotelService>();


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HotelBookingDbContext>();

    // ?? Insertar Hoteles Primero
    if (!context.Hotels.Any())
    {
        context.Hotels.AddRange(
            new Hotel { Name = "Hotel Paradise", Address = "Av. Principal 123, Lima", IsActive = true },
            new Hotel { Name = "Hotel Ocean View", Address = "Calle Mar 456, Miami", IsActive = true }
        );
        context.SaveChanges(); // Guardamos los hoteles antes de continuar
    }

    // ?? Insertar Habitaciones Después
    if (!context.Rooms.Any())
    {
        context.Rooms.AddRange(
            new Room { HotelId = 1, BasePrice = 100.00m, Taxes = 18.00m, IsActive = true },
            new Room { HotelId = 1, BasePrice = 120.00m, Taxes = 21.00m, IsActive = true },
            new Room { HotelId = 2, BasePrice = 80.00m, Taxes = 15.00m, IsActive = true }
        );
        context.SaveChanges(); // Guardamos las habitaciones antes de continuar
    }

    // ?? Insertar Clientes
    // ?? Insertar Clientes
    if (!context.Clients.Any())
    {
        context.Clients.AddRange(
            new Client
            {
                FirstName = "Juan",
                LastName = "Pérez",
                Email = "rfajardomerco@gmail.com",
                PhoneNumber = "987654321",
                DateOfBirth = new DateTime(1990, 5, 15), // Fecha de nacimiento
                Gender = "Masculino",
                DocumentType = "DNI",
                DocumentNumber = "12345678",
                EmergencyContactName = "Carlos Pérez",  // Contacto de emergencia
                EmergencyContactPhone = "987654321"
            },
            new Client
            {
                FirstName = "Ana",
                LastName = "López",
                Email = "rfajardomerco@gmail.com",
                PhoneNumber = "923456789",
                DateOfBirth = new DateTime(1995, 8, 22), // Fecha de nacimiento
                Gender = "Femenino",
                DocumentType = "DNI",
                DocumentNumber = "87654321",
                EmergencyContactName = "María López",  // Contacto de emergencia
                EmergencyContactPhone = "923456789"
            }
        );
        context.SaveChanges(); // Guardamos los clientes antes de continuar
    }


    // ?? Insertar Reservas al Final (Después de que existen Hoteles, Habitaciones y Clientes)
    if (!context.Reservations.Any())
    {
        context.Reservations.AddRange(
            new Reservation { HotelId = 1, RoomId = 1, ClientId = 1, CheckIn = DateTime.Parse("2025-03-15"), CheckOut = DateTime.Parse("2025-03-20"), Status = "Confirmed" },
            new Reservation { HotelId = 2, RoomId = 3, ClientId = 2, CheckIn = DateTime.Parse("2025-04-10"), CheckOut = DateTime.Parse("2025-04-15"), Status = "Pending" }
        );
        context.SaveChanges(); // Guardamos las reservas al final
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
app.UseAuthorization();
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
