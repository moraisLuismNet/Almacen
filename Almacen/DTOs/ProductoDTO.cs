namespace Almacen.DTOs
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = null!;
        public decimal Precio { get; set; }
        public DateOnly? FechaAlta { get; set; }
        public bool Descatalogado { get; set; }
        public string? FotoUrl { get; set; }
        public string NombreCategoria { get; set; } = null!;
    }
}
