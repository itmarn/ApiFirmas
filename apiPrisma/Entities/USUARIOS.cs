namespace apiPrisma.Entities
{
    public class USUARIOS
    {
        public int ID { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Nit {  get; set; }
        public string? Password { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Email { get; set; }
        public int Activo { get; set; }
    }
}
