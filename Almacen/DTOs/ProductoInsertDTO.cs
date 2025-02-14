using Almacen.Validators;

namespace Almacen.DTOs
{
    public class ProductoInsertDTO
    {
        public string NombreProducto { get; set; } = null!;
        public decimal Precio { get; set; }
        public DateOnly? FechaAlta { get; set; }
        public bool Descatalogado { get; set; }

        [PesoArchivoValidacion(PesoMaximoEnMegaBytes: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile? Foto { get; set; }
        public int? CategoriaId { get; set; }

    }
}
