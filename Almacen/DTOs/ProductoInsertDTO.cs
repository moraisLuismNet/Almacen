namespace Almacen.DTOs
{
    public class ProductoInsertDTO
    {
        public string NombreProducto { get; set; } = null!;
        public decimal Precio { get; set; }
        public DateOnly? FechaAlta { get; set; }
        public bool Descatalogado { get; set; }
        public string? FotoUrl { get; set; }
        public int? CategoriaId { get; set; }

    }
}
