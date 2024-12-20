﻿using Almacen.DTOs;
using Almacen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Almacen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AlmacenContext _context;

        public CategoriasController(AlmacenContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoriaDTO>>> GetCategorias()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Productos)
                .ToListAsync();

            var result = categorias.Select(c => new CategoriaDTO
            {
                IdCategoria = c.IdCategoria,
                NombreCategoria = c.NombreCategoria,
                Productos = c.Productos.Select(p => new ProductoDTO
                {
                    IdProducto = p.IdProducto,
                    NombreProducto = p.NombreProducto,
                    Precio = p.Precio,
                    FechaAlta = p.FechaAlta,
                    Descatalogado = p.Descatalogado,
                    FotoUrl = p.FotoUrl
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoriaItemDTO>> GetCategoriaPorId(int id)
        {
            var categoria = await _context.Categorias
                                          
                                          .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
                return NotFound();

            
            var result = new CategoriaItemDTO
            {
                NombreCategoria = categoria.NombreCategoria
               
            };

            return Ok(result);
        }

        [HttpGet("ordenNombreCategoria/{desc}")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetFamiliasOrdenNombre(bool desc)
        {
            List<Categoria> categorias;

            if (desc)
            {
                categorias = await _context.Categorias
                                           .Include(c => c.Productos) 
                                           .OrderBy(x => x.NombreCategoria)
                                           .ToListAsync();
            }
            else
            {
                categorias = await _context.Categorias
                                           .Include(c => c.Productos) 
                                           .OrderByDescending(x => x.NombreCategoria)
                                           .ToListAsync();
            }

            return Ok(categorias);
        }

        [HttpGet("nombreCategoria/contiene/{texto}")]
        public async Task<ActionResult<List<CategoriaDTO>>> GetNombreCategoria(string texto)
        {
            var categorias = await _context.Categorias
                .Where(x => x.NombreCategoria.Contains(texto))
                .Include(x => x.Productos)
                .ToListAsync();

            var categoriasDTO = categorias.Select(c => new CategoriaDTO
            {
                IdCategoria = c.IdCategoria,
                NombreCategoria = c.NombreCategoria,
                Productos = c.Productos.Select(p => new ProductoDTO
                {
                    IdProducto = p.IdProducto,
                    NombreProducto = p.NombreProducto,
                    Precio = p.Precio,
                    FechaAlta = p.FechaAlta,
                    Descatalogado = p.Descatalogado,
                    FotoUrl = p.FotoUrl
                }).ToList()
            }).ToList();

            return categoriasDTO;
        }

        [HttpGet("paginacion/{pagina?}")]
        public async Task<ActionResult> GetCategoriasPaginacion(int pagina = 1)
        {
            int registrosPorPagina = 2;

            var totalCategorias = await _context.Categorias.CountAsync();

            var categorias = await _context.Categorias
                .Include(x => x.Productos)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            var categoriasDTO = categorias.Select(c => new CategoriaDTO
            {
                IdCategoria = c.IdCategoria,
                NombreCategoria = c.NombreCategoria,
                Productos = c.Productos.Select(p => new ProductoDTO
                {
                    IdProducto = p.IdProducto,
                    NombreProducto = p.NombreProducto,
                    Precio = p.Precio,
                    FechaAlta = p.FechaAlta,
                    Descatalogado = p.Descatalogado,
                    FotoUrl = p.FotoUrl
                }).ToList()
            }).ToList();

            var totalPaginas = (int)Math.Ceiling(totalCategorias / (double)registrosPorPagina);

            return Ok(new { categorias = categoriasDTO, totalPaginas });
        }


        [HttpGet("paginacion/{desde}/{hasta}")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasDesdeHasta(int desde, int hasta)
        {
            if (desde < 1)
            {
                return BadRequest("El mínimo debe ser superior a 0");
            }
            if (hasta < desde)
            {
                return BadRequest("El máximo no puede ser inferior al mínimo");
            }

            var categorias = await _context.Categorias
                .Include(c => c.Productos) 
                .Skip(desde - 1)
                .Take(hasta - desde + 1)
                .ToListAsync();

            var categoriasDTO = categorias.Select(c => new CategoriaDTO
            {
                IdCategoria = c.IdCategoria,
                NombreCategoria = c.NombreCategoria,
                Productos = c.Productos.Select(p => new ProductoDTO
                {
                    IdProducto = p.IdProducto,
                    NombreProducto = p.NombreProducto,
                    Precio = p.Precio,
                    FechaAlta = p.FechaAlta,
                    Descatalogado = p.Descatalogado,
                    FotoUrl = p.FotoUrl
                }).ToList()
            }).ToList();

            return Ok(categorias);
        }

        [HttpGet("categoriasProductosSelect/{id:int}")]
        public async Task<ActionResult<Categoria>> GetCategoriasProductosSelect(int id)
        {
            var categoria = await (from x in _context.Categorias
                                 select new CategoriaProductoDTO
                                 {
                                     IdCategoria = x.IdCategoria,
                                     NombreCategoria = x.NombreCategoria,
                                     TotalProductos = x.Productos.Count(),
                                     Productos = x.Productos.Select(y => new ProductoItemDTO
                                     {
                                         IdProducto = y.IdProducto,
                                         NombreProducto = y.NombreProducto
                                     }).ToList(),
                                 }).FirstOrDefaultAsync(x => x.IdCategoria == id);

            if (categoria == null)
            {
                return NotFound();
            }
            return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult> PostCategoria(CategoriaInsertDTO categoria)
        {
            var newCategoria = new Categoria()
            {
                NombreCategoria = categoria.NombreCategoria
            };

            await _context.AddAsync(newCategoria);
            await _context.SaveChangesAsync();

            return Created("Categoria", new { categoria = newCategoria });
        }

        [HttpPut("{idCategoria}")]
        public async Task<IActionResult> PutCategoria(int idCategoria, [FromBody] CategoriaUpdateDTO categoria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (idCategoria != categoria.IdCategoria)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo" });
            }

            var categoriaUpdate = await _context.Categorias
                .AsTracking()
                .FirstOrDefaultAsync(x => x.IdCategoria == idCategoria);

            if (categoriaUpdate == null)
            {
                return NotFound(new { message = "La categoría no fue encontrada" });
            }

            categoriaUpdate.NombreCategoria = categoria.NombreCategoria;
            
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al actualizar la categoría", details = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var hayProductos = await _context.Productos.AnyAsync(x => x.CategoriaId == id);
            if (hayProductos)
            {
                return BadRequest("Hay productos relacionados");
            }
            var categoria = await _context.Categorias.FirstOrDefaultAsync(x => x.IdCategoria == id);

            if (categoria is null)
            {
                return NotFound();
            }

            _context.Remove(categoria);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
