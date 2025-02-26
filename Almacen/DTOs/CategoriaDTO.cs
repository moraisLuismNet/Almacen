namespace Almacen.DTOs
{
    public class CategoriaDTO
    {
        public int IdCategoria { get; set; }

        public string NombreCategoria { get; set; } = null!;

        public List<ProductoDTO> Productos { get; set; } = new();
        public int TotalProductos { get; internal set; }
    }
}
