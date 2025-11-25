using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace apiPrisma.Clases
{
    public class Peticion
    {
        public int sistema {  get; set; }
        public string? archivo64 { get; set;}
        public string? rubrica64 { get; set; }
        public string? path_firmado { get; set; }
        public string concepto { get; set; }
        public string lugar { get; set; }
        public int pagina { get; set; } 
        public string ? coordenadas { get; set; }
    }
}
