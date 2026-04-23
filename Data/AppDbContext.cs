using BlazorServerCPP.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace BlazorServerCPP.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Clienti> Clienti { get; set; }
    public DbSet<Impianti> Impianti { get; set; }
    public DbSet<Licenze> Licenze { get; set; }
    public DbSet<Commesse> Commesse { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.Id);
            e.Property(u => u.Username).HasMaxLength(64).IsRequired();
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Clienti>()
            .ToTable("clienti");

        modelBuilder.Entity<Impianti>(entity =>
        {
            entity.ToTable("impianti");

            entity.HasKey(e => e.IdImpianto);

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Impiantis)
                .HasForeignKey(e => e.IdCliente)
                .HasPrincipalKey(c => c.IdCliente);

            entity.HasOne(e => e.Commessa)
                .WithMany()
                .HasForeignKey(e => e.IdCommessa)
                .HasPrincipalKey(c => c.IdCommessa);
        });

        modelBuilder.Entity<Licenze>(entity =>
        {
            entity.ToTable("licenze");

            entity.HasKey(e => e.IdLicenza);

            // relazione Cliente
            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Licenzes)
                .HasForeignKey(e => e.IdCliente)
                .HasPrincipalKey(c => c.IdCliente);

            // relazione Impianto
            entity.HasOne(e => e.Impianto)
                .WithMany(i => i.Licenzes)
                .HasForeignKey(e => e.IdImpianto)
                .HasPrincipalKey(i => i.IdImpianto);
        });
    }
}
