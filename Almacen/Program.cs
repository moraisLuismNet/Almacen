using Almacen.Models;
using Almacen.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Esta opci�n es para evitar referencias circulares al utilizar include en los controllers
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
                            builder.Configuration.GetConnectionString("DefaultConnection");

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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Middleware para acceder a archivos est�ticos de la carpeta wwwroot (Ponerlo antes de app.Run() - 12:20
app.UseStaticFiles();

app.Run();
