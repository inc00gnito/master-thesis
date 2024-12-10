using Course.API.Consumers;
using Course.API.Data;
using Course.API.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CourseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(Program));

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

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<DataSeeder>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CourseContext>();
    db.Database.Migrate();
    // Get the seeder and run it after migration
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedData();
}

app.Run();