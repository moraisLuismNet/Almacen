namespace Almacen.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public byte[]? Salt { get; set; }
    }
}
