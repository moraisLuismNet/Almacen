using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Almacen.Filters
{
    public class FiltroDeExcepcion : IExceptionFilter
    {
        private readonly ILogger<FiltroDeExcepcion> _logger;
        private readonly IWebHostEnvironment _env;

        public FiltroDeExcepcion(ILogger<FiltroDeExcepcion> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);

            var path = $@"{_env.ContentRootPath}\wwwroot\errores.txt";
            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {context.Exception.Message}");
            }

            context.Result = new StatusCodeResult(500);
            context.ExceptionHandled = true;
        }

    }
}
