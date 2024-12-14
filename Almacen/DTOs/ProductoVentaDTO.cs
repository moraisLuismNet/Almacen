namespace Almacen.DTOs
{
    public class ProductoVentaDTO
    {
        public string NombreProducto { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal? Precio { get; set; }
        public string NombreCategoria { get; set; } = null!;
    }
}
