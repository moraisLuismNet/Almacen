using Almacen.DTOs;
using Almacen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Almacen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : Controller
    {
        private readonly AlmacenContext _context;

        public ProductosController(AlmacenContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductoDTO>>> GetProductos()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)  
                .ToListAsync();

            var productosDTO = productos.Select(p => new ProductoDTO
            {
                IdProducto = p.IdProducto,
                NombreProducto = p.NombreProducto,
                Precio = p.Precio,
                FechaAlta = p.FechaAlta,
                Descatalogado = p.Descatalogado,
                FotoUrl = p.FotoUrl,
                NombreCategoria = p.Categoria.NombreCategoria
            }).ToList();
            return Ok(productosDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoDTO>> GetProductoPorId(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)  
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            var productoDTO = new ProductoDTO
            {
                IdProducto = producto.IdProducto,
                NombreProducto = producto.NombreProducto,
                Precio = producto.Precio,
                FechaAlta = producto.FechaAlta,
                Descatalogado = producto.Descatalogado,
                FotoUrl = producto.FotoUrl,
                NombreCategoria = producto.Categoria.NombreCategoria  
            };

            return Ok(productoDTO);
        }

        [HttpGet("ordenNombreProducto/{desc}")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductosOrdenNombre(bool desc)
        {
            List<ProductoDTO> productosDTO = new List<ProductoDTO>();

            List<Producto> productos;
            if (desc)
            {
                productos = await _context.Productos
                    .OrderBy(x => x.NombreProducto)
                    .Include(p => p.Categoria) 
                    .ToListAsync();
            }
            else
            {
                productos = await _context.Productos
                    .OrderByDescending(x => x.NombreProducto)
                    .Include(p => p.Categoria) 
                    .ToListAsync();
            }

            productosDTO = productos.Select(p => new ProductoDTO
            {
                IdProducto = p.IdProducto,
                NombreProducto = p.NombreProducto,
                Precio = p.Precio,
                FechaAlta = p.FechaAlta,
                Descatalogado = p.Descatalogado,
                FotoUrl = p.FotoUrl,
                NombreCategoria = p.Categoria.NombreCategoria 
            }).ToList();

            return Ok(productosDTO);
        }

        [HttpGet("nombreProducto/contiene/{texto}")]
        public async Task<ActionResult<List<ProductoDTO>>> GetNombreProducto(string texto)
        {
            var productos = await _context.Productos
                .Where(x => x.NombreProducto.Contains(texto))
                .Include(p => p.Categoria) 
                .ToListAsync();

            var productosDTO = productos.Select(p => new ProductoDTO
            {
                IdProducto = p.IdProducto,
                NombreProducto = p.NombreProducto,
                Precio = p.Precio,
                FechaAlta = p.FechaAlta,
                Descatalogado = p.Descatalogado,
                FotoUrl = p.FotoUrl,
                NombreCategoria = p.Categoria.NombreCategoria 
            }).ToList();

            return Ok(productosDTO);
        }

        [HttpGet("precio/entre")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductosByPrecios([FromQuery] decimal min, [FromQuery] decimal max)
        {
            var productos = await _context.Productos
                .Where(x => x.Precio > min && x.Precio < max)
                .Include(p => p.Categoria) 
                .ToListAsync();

            var productosDTO = productos.Select(p => new ProductoDTO
            {
                IdProducto = p.IdProducto,
                NombreProducto = p.NombreProducto,
                Precio = p.Precio,
                FechaAlta = p.FechaAlta,
                Descatalogado = p.Descatalogado,
                FotoUrl = p.FotoUrl,
                NombreCategoria = p.Categoria.NombreCategoria 
            }).ToList();

            return Ok(productosDTO);
        }

        [HttpGet("paginacion/{pagina?}")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductosPaginacion(int pagina = 1)
        {
            int registrosPorPagina = 5;
            var productos = await _context.Productos
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Include(p => p.Categoria) 
                .ToListAsync();

            var productosDTO = productos.Select(p => new ProductoDTO
            {
                IdProducto = p.IdProducto,
                NombreProducto = p.NombreProducto,
                Precio = p.Precio,
                FechaAlta = p.FechaAlta,
                Descatalogado = p.Descatalogado,
                FotoUrl = p.FotoUrl,
                NombreCategoria = p.Categoria.NombreCategoria 
            }).ToList();

            return Ok(productosDTO);
        }

        [HttpGet("paginacion/{desde}/{hasta}")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductosDesdeHasta(int desde, int hasta)
        {
            if (desde < 1)
            {
                return BadRequest("El mínimo debe ser superior a 0");
            }
            if (hasta < desde)
            {
                return BadRequest("El máximo no puede ser inferior al mínimo");
            }

            var productos = await _context.Productos
                .Skip(desde - 1)
                .Take(hasta - desde)
                .Include(p => p.Categoria) 
                .ToListAsync();

            var productosDTO = productos.Select(p => new ProductoDTO
            {
                IdProducto = p.IdProducto,
                NombreProducto = p.NombreProducto,
                Precio = p.Precio,
                FechaAlta = p.FechaAlta,
                Descatalogado = p.Descatalogado,
                FotoUrl = p.FotoUrl,
                NombreCategoria = p.Categoria.NombreCategoria 
            }).ToList();

            return Ok(productosDTO);
        }

        [HttpGet("productoVenta")]
        public async Task<ActionResult<IEnumerable<ProductoVentaDTO>>> GetProductosYPrecios()
        {
            var productos = await _context.Productos
                .Include(x => x.Categoria) 
                .Select(x => new ProductoVentaDTO
                {
                    NombreProducto = x.NombreProducto,
                    Precio = x.Precio,
                    NombreCategoria = x.Categoria.NombreCategoria 
                })
                .ToListAsync();

            return Ok(productos);
        }

        [HttpGet("productosAgrupadosPorDescatalogado")]
        public async Task<ActionResult> GetProductosAgrupadosPorDescatalogado()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria)  
                .GroupBy(g => g.Descatalogado)
                .Select(x => new
                {
                    Descatalogado = x.Key,
                    Total = x.Count(),
                    Productos = x.Select(p => new
                    {
                        p.IdProducto,
                        p.NombreProducto,
                        p.Precio,
                        p.FechaAlta,
                        p.Descatalogado,
                        p.FotoUrl,
                        p.Categoria.NombreCategoria 
                    }).ToList()
                }).ToListAsync();

            return Ok(productos);
        }

        // Consulta diferida
        [HttpGet("filtrar")]
        public async Task<ActionResult> GetFiltroMultiple([FromQuery] ProductoFiltroDTO filtroProductos)
        {
            /* AsQueryable nos permite ir construyendo paso a paso el filtrado y ejecutarlo 
            al final. Si lo convertimos a una lista (toListAsync) el resto de filtros los hacemos 
            en memoria porque toListAsync ya trae a la memoria del servidor los datos desde el 
            servidor de base de datos. Hacer los filtros en memoria es menos eficiente que hacerlos 
            en una base de datos. Construimos los filtros de forma dinámica y hasta que no hacemos el 
            ToListAsync no vamos a la base de datos para traer la información. */
            var productosQueryable = _context.Productos
                .Include(p => p.Categoria) 
                .AsQueryable(); 

            if (!string.IsNullOrEmpty(filtroProductos.NombreProducto))
            {
                productosQueryable = productosQueryable.Where(x => x.NombreProducto.Contains(filtroProductos.NombreProducto));
            }

            if (filtroProductos.Descatalogado)
            {
                productosQueryable = productosQueryable.Where(x => x.Descatalogado);
            }

            if (filtroProductos.CategoriaId != 0)
            {
                productosQueryable = productosQueryable.Where(x => x.CategoriaId == filtroProductos.CategoriaId);
            }

            var productosDTO = await productosQueryable
                .Select(p => new ProductoDTO
                {
                    IdProducto = p.IdProducto,
                    NombreProducto = p.NombreProducto,
                    Precio = p.Precio,
                    FechaAlta = p.FechaAlta,
                    Descatalogado = p.Descatalogado,
                    FotoUrl = p.FotoUrl,
                    NombreCategoria = p.Categoria.NombreCategoria 
                })
                .ToListAsync();

            return Ok(productosDTO);
        }

        [HttpPost]
        public async Task<ActionResult> PostProducto(ProductoInsertDTO producto)
        {
            var newProducto = new Producto()
            {
                NombreProducto = producto.NombreProducto,
                Precio = producto.Precio,
                FechaAlta = producto.FechaAlta,
                Descatalogado = producto.Descatalogado,
                FotoUrl = producto.FotoUrl,
                CategoriaId = (int)producto.CategoriaId
            };

            await _context.AddAsync(newProducto);
            await _context.SaveChangesAsync();

            return Created("Producto", new { producto = newProducto });
        }

        [HttpPut("{idProducto}")]
        public async Task<IActionResult> PutProducto(int idProducto, [FromBody] ProductoUpdateDTO producto)
        {
            if (idProducto != producto.IdProducto)
            {
                return BadRequest(new { message = "El ID del producto no coincide con el cuerpo de la solicitud" });
            }

            var productoUpdate = await _context.Productos
                .AsTracking()
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto);

            if (productoUpdate == null)
            {
                return NotFound(new { message = "El producto no fue encontrado" });
            }

            if (producto.CategoriaId != 0)
            {
                var categoria = await _context.Categorias.FindAsync(producto.CategoriaId);
                if (categoria == null)
                {
                    return BadRequest(new { message = "La categoría proporcionada no existe" });
                }
                productoUpdate.CategoriaId = (int)producto.CategoriaId; 
            }

            productoUpdate.NombreProducto = producto.NombreProducto;
            productoUpdate.Precio = producto.Precio;
            productoUpdate.FechaAlta = producto.FechaAlta;
            productoUpdate.Descatalogado = producto.Descatalogado;
            productoUpdate.FotoUrl = producto.FotoUrl;
            productoUpdate.CategoriaId = (int)producto.CategoriaId;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); 
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al actualizar el producto", details = ex.Message });
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(x => x.IdProducto == id);

            if (producto is null)
            {
                return NotFound();
            }

            _context.Remove(producto);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
