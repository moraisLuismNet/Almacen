namespace Almacen.DTOs
{
    public class CategoriaProductoDTO
    {
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public int TotalProductos { get; set; }
        public List<ProductoItemDTO> Productos { get; set; }
    }
    public class ProductoItemDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
    }
}
