using Almacen.Models;

namespace Almacen.Services
{
    public class ActionsService
    {
        private readonly AlmacenContext _context;
        private readonly IHttpContextAccessor _accessor;

        public ActionsService(AlmacenContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public async Task AddAction(string accion, string controller)
        {
            Almacen.Models.Action nuevaAccion = new()
            {
                FechaAccion = DateTime.Now,
                Accion = accion,
                Controller = controller,
                Ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString()
            };

            await _context.Actions.AddAsync(nuevaAccion);
            await _context.SaveChangesAsync();

            Task.FromResult(0);
        }
    }

 }
