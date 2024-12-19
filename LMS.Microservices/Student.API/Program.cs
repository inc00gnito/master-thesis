using MassTransit;
using Microsoft.EntityFrameworkCore;
using Student.API.Consumers;
using Student.API.Data;
using Student.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<StudentContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    // Add consumers
    x.AddConsumer<EnrollmentCreatedConsumer>();
    x.AddConsumer<EnrollmentCancelledConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        // Configure consumers
        cfg.ConfigureEndpoints(context);
    });
});

// Add Services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<DataSeeder>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

// Apply migrations
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<StudentContext>();
        if (db.Database.GetPendingMigrations().Any())
        {
            db.Database.Migrate();
        }
        // Get the seeder and run it after migration
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedData();
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating the database.");
}

app.Run();