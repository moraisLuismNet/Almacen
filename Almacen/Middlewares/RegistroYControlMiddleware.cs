namespace Almacen.Middlewares
{
    public class RegistroYControlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public RegistroYControlMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var IP = httpContext.Connection.RemoteIpAddress?.ToString();
            var ruta = httpContext.Request.Path.ToString();
            var metodo = httpContext.Request.Method;
            var fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var path = Path.Combine(_env.ContentRootPath, "wwwroot", "log.txt");

            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                writer.WriteLine($"{fechaHora} | IP: {IP} | Ruta: {ruta} | Método: {metodo}");
            }

            await _next(httpContext);
        }
    }
}
