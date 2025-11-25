using apiPrisma.Entities;
using Microsoft.EntityFrameworkCore;
namespace apiPrisma.Context
{
    public class AppDbContextSeg : DbContext
    {
        public AppDbContextSeg(DbContextOptions<AppDbContextSeg> options) : base(options)
        {

        }
        public DbSet<USUARIOS> Parametros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<USUARIOS>(tb => {
                tb.HasKey(col => col.ID);
                tb.ToTable("USUARIOS", "MARN_SISTEMAS");
            });
        }
    }
}
