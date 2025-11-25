using apiPrisma.Context;
using apiPrisma.Entities;
using Microsoft.EntityFrameworkCore;

namespace apiPrisma.Services
{
    public class UsuariosService
    {
        private readonly AppDbContextSeg _context;

        public UsuariosService(AppDbContextSeg context)
        {
            _context = context;
        }

        public async Task<IList<USUARIOS>> lista()
        {
            var listaDB = await _context.Parametros.ToListAsync();
            return listaDB;
        }

        public async Task<USUARIOS> buscar(string NombreUsuario)
        {
            //PARCHE COMPATIBILIDAD 11G
            var listaDB = await _context.Parametros.Where(i => i.NombreUsuario.ToUpper().Trim() == NombreUsuario.ToUpper().Trim()).ToListAsync();
            return listaDB[0];            
        }
    }
}
