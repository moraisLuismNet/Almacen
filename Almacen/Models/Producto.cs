using System;
using System.Collections.Generic;

namespace Almacen.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string NombreProducto { get; set; } = null!;

    public decimal Precio { get; set; }

    public DateOnly? FechaAlta { get; set; }

    public bool Descatalogado { get; set; }

    public string? FotoUrl { get; set; }

    public int CategoriaId { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;
}
