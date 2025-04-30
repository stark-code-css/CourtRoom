using CourtRoom.Models;
using Microsoft.EntityFrameworkCore;

namespace CourtRoom.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<CourtOrder> CourtOrders { get; set; }
    public DbSet<PaymentDetail> PaymentDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CourtOrder>()
            .HasOne(c => c.PaymentDetail)
            .WithOne(p => p.CourtOrder)
            .HasForeignKey<PaymentDetail>(p => p.CourtOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CourtOrder>()
            .HasIndex(c => c.CaseNo)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}