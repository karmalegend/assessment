using System.Text.Json.Serialization;
using ApI.Middleware;
using Data;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    setup.AddSecurityDefinition(ApiKeyAuthenticationOptions.DefaultScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = ApiKeyAuthenticationOptions.HeaderName,
        Type = SecuritySchemeType.ApiKey
    });

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = ApiKeyAuthenticationOptions.DefaultScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

#region Scoped

builder.Services.AddScoped<ApiKeyAuthenticationHandler>();
builder.Services.AddScoped<IGarageRepository, GarageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IParkingSessionRepository, ParkingSessionRepository>();

#endregion

#region Transient

builder.Services.AddTransient<IGarageManagementService, GarageManagementService>();
builder.Services.AddTransient<IUserManagementService, UserManagementService>();
builder.Services.AddTransient<IParkingSessionService, ParkingSessionService>();

#endregion

// builder.Services.AddDbContext<ParkingDbContext>(options =>
//     options.UseSqlServer("Server=localhost,1433;Database=ParkingDb;User Id=SA;Password=YourPassword1;"));;
builder.Services.AddDbContext<ParkingDbContext>(options => options.UseInMemoryDatabase("parkbee"));

builder.Services.AddAuthentication(ApiKeyAuthenticationOptions.DefaultScheme)
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme,
        null);


var app = builder.Build();

app.UseMiddleware<DomainExceptionHandlingMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

AddSeedData(app);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void AddSeedData(WebApplication app)
{
    var scope = app.Services.CreateScope();
    var parkingDbContext = scope.ServiceProvider.GetService<ParkingDbContext>();

    if (parkingDbContext == null) throw new ArgumentNullException(nameof(parkingDbContext));
    parkingDbContext.Database.EnsureCreated();

    parkingDbContext.Garages.AddRange(Enumerable.Range(0, 10).Select(x => new Garage
    {
        Id = Guid.NewGuid(),
        Name = $"Garage {Guid.NewGuid()}",
        ParkingSpotsAvailable = 300,
        Doors = new List<Door>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Description = $"Door {Guid.NewGuid()}",
                DoorType = DoorType.Entry,
                IpAddress = new IpAddress("142.250.179.206") // google
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = $"Door {Guid.NewGuid()}",
                DoorType = DoorType.Exit,
                IpAddress = new IpAddress("18.239.50.28") // parkbee.com
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = $"Door {Guid.NewGuid()}",
                DoorType = DoorType.Pedestrian,
                IpAddress = new IpAddress("52.142.124.215") // duckduckgo
            }
        }
    }));

    parkingDbContext.Users.AddRange(Enumerable.Range(0, 20).Select(x => new User
    {
        Id = Guid.NewGuid(),
        PartnerId = "partner-1",
        LicensePlate = GenerateRandomLicensePlate()
    }));

    // the user we're pretending to be.
    parkingDbContext.Users.Add(new User
    {
        Id = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
        PartnerId = "partner-1",
        LicensePlate = GenerateRandomLicensePlate()
    });

    parkingDbContext.SaveChanges();
}

static string GenerateRandomLicensePlate()
{
    const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string digits = "0123456789";
    var random = new Random();

    return new string(Enumerable.Range(0, 2).Select(_ => letters[random.Next(letters.Length)]).ToArray()) + "-" +
           new string(Enumerable.Range(0, 3).Select(_ => digits[random.Next(digits.Length)]).ToArray()) + "-" +
           new string(Enumerable.Range(0, 2).Select(_ => letters[random.Next(letters.Length)]).ToArray());
}
