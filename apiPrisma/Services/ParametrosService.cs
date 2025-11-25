using apiPrisma.Context;
using apiPrisma.Entities;
using Microsoft.EntityFrameworkCore;

namespace apiPrisma.Services
{
    public class ParametrosService
    {

        private readonly AppDbContext _context;

        public ParametrosService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IList<TB_PARAMETROS_SISTEMA>> lista()
        {
            var listaDB = await _context.Parametros.ToListAsync();
            return listaDB;
        }

        public async Task<TB_PARAMETROS_SISTEMA> buscar(int id)
        {
            //PROBLEMAS COMPATIBILIDAD 11G
            TB_PARAMETROS_SISTEMA parametro = await _context.Parametros.FirstOrDefaultAsync(x=> x.ID_SISTEMA == id);
            return parametro;

            //var listaDB = await _context.Parametros.Where(i => i.ID_SISTEMA == id).ToListAsync();
            //return listaDB[0];
        }



        //public async Task pruebabd()
        //{
        //    await _context.Database.ExecuteSqlRawAsync("UPDATE TB_PARAMETROS_SISTEMA SET CONCEPTO = 'PRUEBA DE MODIFICACION' WHERE ID_PARAMETRO = 2 ");
        //}

    }
}
