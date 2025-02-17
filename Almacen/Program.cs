using Almacen.Filters;
using Almacen.Middlewares;
using Almacen.Models;
using Almacen.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAlmacen.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Esta opción es para evitar referencias circulares al utilizar include en los controllers
//builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(FiltroDeExcepcion));

}).AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<AlmacenContext>(options =>
{
    options.UseSqlServer(connectionString);
    //Deshabilitar el tracking
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
}
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ActionsService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IGestorArchivosService, GestorArchivosService>();
builder.Services.AddHostedService<TareaProgramadaService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddDataProtection();
builder.Services.AddTransient<HashService>();
builder.Services.AddTransient<TokenService>();

// Configurar seguridad
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(
                     Encoding.UTF8.GetBytes(builder.Configuration["ClaveJWT"]))
               });


// Configurar la seguridad en Swagger 
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "Autenticación JWT usando el esquema Bearer. \r\n\r " +
        "Ingresa la palabra 'Bearer' seguido de un espacio y el token de autenticación",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
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
                        new string[]{}
                    }
                });

});

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)  // Leer configuración desde appsettings.json
    .CreateLogger();

// Usar Serilog como el proveedor de logs en la aplicación
builder.Host.UseSerilog();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.UseMiddleware<RegistroYControlMiddleware>();

app.MapControllers();

// Middleware para acceder a archivos estáticos de la carpeta wwwroot 
app.UseStaticFiles();

app.Run();
