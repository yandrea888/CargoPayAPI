using CargoPayAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CargoPayAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>()
                .HasIndex(c => c.CardNumber)
                .IsUnique(); 
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Card)
                .WithMany()  
                .HasForeignKey(p => p.CardId)
                .OnDelete(DeleteBehavior.Cascade); 

            base.OnModelCreating(modelBuilder);
        }
    }


}
