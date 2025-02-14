using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Almacen.Models;


namespace Almacen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionsController : Controller
    {
        private readonly AlmacenContext _context;

        public ActionsController(AlmacenContext context)
        {
            _context = context;
        }

        // GET: api/Actions
        [HttpGet]
        public async Task<ActionResult> GetActions()
        {
            var acciones = await _context.Actions.ToListAsync();
            return Ok(acciones);
        }

    }
}
