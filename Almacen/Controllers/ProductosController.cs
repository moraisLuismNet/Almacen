﻿using Almacen.DTOs;
using Almacen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Almacen.Services;
using Microsoft.AspNetCore.Authorization;

namespace Almacen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : Controller
    {
        private readonly AlmacenContext _context;
        private readonly ActionsService _actionsService;
        private readonly IGestorArchivosService _gestorArchivosService;


        public ProductosController(AlmacenContext context, ActionsService actionsService, IGestorArchivosService gestorArchivosService)
        {
            _context = context;
            _actionsService = actionsService;
            _gestorArchivosService = gestorArchivosService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductoDTO>>> GetProductos()
        {
            await _actionsService.AddAction("Obtener productos", "Productos");
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
            await _actionsService.AddAction("Obtener un producto por id", "Productos");
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
            await _actionsService.AddAction("Obtener productos por orden (nombre)", "Productos");
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
            await _actionsService.AddAction("Obtener productos que contienen (nombre)", "Productos");
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
            await _actionsService.AddAction("Obtener productos con un precio entre", "Productos");
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
            await _actionsService.AddAction("Obtener productos paginados", "Productos");
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
            await _actionsService.AddAction("Obtener productos paginados desde|hasta", "Productos");
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
            await _actionsService.AddAction("Obtener productos y precios", "Productos");
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
            await _actionsService.AddAction("Obtener productos descatalogados", "Productos");
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
            await _actionsService.AddAction("Obtener productos con un filtro múltiple", "Productos");
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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> PostProducto(ProductoInsertDTO producto)
        {
            await _actionsService.AddAction("Añadir productos", "Productos");
            var newProducto = new Producto()
            {
                NombreProducto = producto.NombreProducto,
                Precio = producto.Precio,
                //FechaAlta = producto.FechaAlta,
                FechaAlta = DateOnly.FromDateTime(DateTime.Now),
                Descatalogado = producto.Descatalogado,
                //Descatalogado = false,
                FotoUrl = "",
                CategoriaId = (int)producto.CategoriaId
            };

            if (producto.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Extraemos la imagen de la petición
                    await producto.Foto.CopyToAsync(memoryStream);
                    // La convertimos a un array de bytes que es lo que necesita el método de guardar
                    var contenido = memoryStream.ToArray();
                    // La extensión la necesitamos para guardar el archivo
                    var extension = Path.GetExtension(producto.Foto.FileName);
                    // Recibimos el nombre del archivo
                    // El servicio Transient GestorArchivos instancia el servicio y cuando se deja de usar se destruye
                    newProducto.FotoUrl = await _gestorArchivosService.GuardarArchivo(contenido, extension, "img",
                        producto.Foto.ContentType);
                }
            }

            await _context.AddAsync(newProducto);
            await _context.SaveChangesAsync();
            //return Ok(newProducto);
            return Created("Producto", new { producto = newProducto });
        }

        [Authorize]
        [HttpPut("{idProducto:int}")]
        public async Task<IActionResult> PutProducto(int idProducto, [FromForm] ProductoUpdateDTO producto)
        {
            await _actionsService.AddAction("Actualiza un producto", "Productos");
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

            if (producto.Foto != null)
            {
                using var memoryStream = new MemoryStream();
                await producto.Foto.CopyToAsync(memoryStream);
                var contenido = memoryStream.ToArray();
                var extension = Path.GetExtension(producto.Foto.FileName);
                var contentType = producto.Foto.ContentType;

                var rutaImagen = await _gestorArchivosService.GuardarArchivo(contenido, extension, "img", contentType);
                productoUpdate.FotoUrl = rutaImagen;
            }

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

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _actionsService.AddAction("Elimina un producto", "Productos");
            var producto = await _context.Productos.FirstOrDefaultAsync(x => x.IdProducto == id);

            if (producto is null)
            {
                return NotFound();
            }

            await _gestorArchivosService.BorrarArchivo(producto.FotoUrl, "img");
            _context.Remove(producto);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
