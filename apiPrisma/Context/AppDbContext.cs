using apiPrisma.Entities;
using Microsoft.EntityFrameworkCore;
namespace apiPrisma.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<TB_PARAMETROS_SISTEMA> Parametros { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TB_PARAMETROS_SISTEMA>(tb => {
                tb.HasKey(col => col.ID_SISTEMA);
                tb.ToTable("TB_PARAMETROS_SISTEMA");
            });            
        }

    }
}
