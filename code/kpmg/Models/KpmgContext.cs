using Microsoft.EntityFrameworkCore;

namespace KpmgApi.Models
{
    public class KpmgContext : DbContext
    {
        public KpmgContext(DbContextOptions<KpmgContext> options)
            : base(options)
        {
        }

        public DbSet<KpmgItem> KpmgItems { get; set; }
        public DbSet<Rate> Rate { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KpmgItem>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.HasMany(r => r.Data)
                    .WithOne(p => p.KpmgItem)
                    .HasForeignKey(f => f.KpmgCode)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Rate>(entity =>
            {
                entity.HasKey(e => new { e.KpmgCode, e.Date });


            });
        }
    }
}
