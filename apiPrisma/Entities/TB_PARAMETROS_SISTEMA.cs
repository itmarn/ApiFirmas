namespace apiPrisma.Entities
{
    public class TB_PARAMETROS_SISTEMA
    {
        public int ID_PARAMETRO { get; set; }
        public int ID_SISTEMA { get; set; }
        public string? COORDENADAS { get; set; }
        public string? LLAVE_ACCESO { get; set; }
        public string? EMAIL { get; set; }
        public string? PASSWORD_FIRMA { get; set; }
        public string? FONT_SIZE { get; set; }
    }
}
