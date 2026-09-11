using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SB.PruebaTecnica.API.Middleware;
using SB.PruebaTecnica.Application.Common;
using SB.PruebaTecnica.Infrastructure;
using SB.PruebaTecnica.Infrastructure.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ---------- Serilog ----------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "Logs", "log-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14)
    .CreateLogger();

builder.Host.UseSerilog();

// ---------- Servicios ----------
builder.Services.AddControllers() .AddJsonOptions(options => { 
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()); 
    }
);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// CORS: permite que el frontend React (Maqueta.jpg) consuma la API.
const string PoliticaCorsFrontend = "PoliticaCorsFrontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(PoliticaCorsFrontend, policy =>
    {
        var origenesPermitidos = builder.Configuration
            .GetSection("Cors:OrigenesPermitidos").Get<string[]>() ?? new[] { "http://localhost:5173" };

        policy.WithOrigins(origenesPermitidos)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---------- Autenticación JWT (Authorization Bearer) ----------
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSecreto = jwtSection["Secreto"] ?? throw new InvalidOperationException("Falta configurar Jwt:Secreto en AppSettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Emisor"],
        ValidAudience = jwtSection["Audiencia"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecreto))
    };
});

builder.Services.AddAuthorization();

// ---------- Swagger ----------
//---------- http://localhost:5099/swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SB - API Prueba Técnica",
        Version = "v1",
        Description = "API RESTful para gestión de pagos de empleados y mantenimiento de entidades gubernamentales de la República Dominicana."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT precedido de la palabra 'Bearer', ej: Bearer eyJhbGci..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ---------- Pipeline HTTP ----------
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SB - API Prueba Técnica v1");
    });
}

app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment()) 
{ 
    app.UseHttpsRedirection(); 
}

app.UseStaticFiles();

app.UseCors(PoliticaCorsFrontend);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await DbInitializer.InicializarAsync(app.Services);

try
{
    Log.Information("Iniciando SB.PruebaTecnica.API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación terminó de forma inesperada");
}
finally
{
    Log.CloseAndFlush();
}
