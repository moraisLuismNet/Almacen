namespace Almacen.DTOs
{
    public class ProductoFiltroDTO
    {
        public string? NombreProducto { get; set; }
        public int CategoriaId { get; set; }
        public bool Descatalogado { get; set; }
    }
}
