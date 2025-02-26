using Microsoft.EntityFrameworkCore;
using Almacen.Models;

namespace Almacen.Services
{
    public class TareaProgramadaService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _env;
        private readonly string nombreArchivo = "ProductosDescatalogados.txt";

        public TareaProgramadaService(IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            _serviceProvider = serviceProvider;
            _env = env;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AlmacenContext>();

                    // Contar los productos descatalogados (Descontinuado = true)
                    int cantidadDescatalogados = await context.Productos.CountAsync(p => p.Descatalogado);

                    // Escribir en el archivo
                    Escribir($"Productos descatalogados: {cantidadDescatalogados}");
                }
            }
            catch (Exception ex)
            {
                Escribir($"Error al contar productos descatalogados: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        
        private void Escribir(string mensaje)
        {
            var ruta = $@"{_env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine($"{DateTime.Now}: {mensaje}");
            }
        }
    }
}
