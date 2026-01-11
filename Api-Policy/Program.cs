using Api_Policy.Data;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Threading.RateLimiting;


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

// Configurar Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // Política global: 100 requests por minuto
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Política específica para crear pólizas: 10 requests por minuto por IP
    options.AddFixedWindowLimiter("CreatePolicy", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });

    // Política para consultas: 50 requests por minuto por IP
    options.AddFixedWindowLimiter("GetPolicies", options =>
    {
        options.PermitLimit = 50;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    });

    // Mensaje personalizado cuando se excede el límite
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Demasiadas solicitudes",
            message = "Has excedido el límite de solicitudes. Por favor, intenta nuevamente más tarde.",
            retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) ? (double?)retryAfter.TotalSeconds : null
        }, cancellationToken: token);
    };
});

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

app.UseRateLimiter(); // Activar rate limiting

app.UseCors("AllowQuasar");

app.MapControllers();

app.Run();


