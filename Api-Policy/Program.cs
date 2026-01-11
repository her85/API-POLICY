using Api_Policy.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Policy",
        Version = "v1",
        Description = "API para gestión de pólizas de seguro y siniestros"
    });
});

// Configurar SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar el servicio de precios que creamos antes
builder.Services.AddScoped<IPricingService, PricingService>();

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:9000" };
    options.AddPolicy("AllowQuasar",
        policy => policy.WithOrigins(allowedOrigins) // Orígenes desde appsettings.json
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// Ejecutar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate(); // Aplica migraciones pendientes
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al ejecutar las migraciones de la base de datos.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Policy v1");
        options.RoutePrefix = string.Empty; // Swagger UI en la raíz (http://localhost:xxxx/)
    });
}

// app.UseHttpsRedirection(); // Deshabilitado para desarrollo

app.UseCors("AllowQuasar");

app.MapControllers();

app.Run();


