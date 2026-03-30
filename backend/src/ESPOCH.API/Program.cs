using System.Text;
using ESPOCH.Core.Interfaces;
using ESPOCH.Core.Services;
using ESPOCH.Infrastructure.Data;
using ESPOCH.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Railway uses PORT env variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Database - PostgreSQL
builder.Services.AddDbContext<ESPOCHDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();
builder.Services.AddScoped<IHorarioRepository, HorarioRepository>();
builder.Services.AddScoped<IAsistenciaRepository, AsistenciaRepository>();
builder.Services.AddScoped<IAusenciaRepository, AusenciaRepository>();

// Services
builder.Services.AddSingleton<IGeolocationService, GeolocationService>();
builder.Services.AddScoped<IAsistenciaService, AsistenciaService>();
builder.Services.AddScoped<IAusenciaService, AusenciaService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "ESPOCH-Default-Secret-Key-Change-In-Production-32chars"))
        };
    });

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("JefeDirecto", policy => policy.RequireRole("Admin", "JefeDirecto"));
    options.AddPolicy("Colaborador", policy => policy.RequireRole("Admin", "JefeDirecto", "Colaborador"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "ESPOCH API", 
        Version = "v1",
        Description = "API para la Plataforma de Experiencia del Colaborador"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS - allow all origins for Railway deployment
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Always enable Swagger (including production on Railway)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ESPOCH API v1");
    c.RoutePrefix = "swagger";
});

// Redirect root to swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created (graceful - won't crash if DB unavailable)
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ESPOCHDbContext>();
    dbContext.Database.EnsureCreated();
    Console.WriteLine("Database connection successful and schema ensured.");
}
catch (Exception ex)
{
    Console.WriteLine($"WARNING: Could not connect to database: {ex.Message}");
    Console.WriteLine("The app will start but database operations will fail until a valid connection string is configured.");
}

app.Run();
