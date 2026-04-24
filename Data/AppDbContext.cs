using BlazorServerCPP.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerCPP.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Utenti> Utenti => Set<Utenti>();
    public DbSet<Clienti> Clienti { get; set; }
    public DbSet<Impianti> Impianti { get; set; }
    public DbSet<Licenze> Licenze { get; set; }
    public DbSet<Commesse> Commesse { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Utenti>(entity =>
        {
            entity.ToTable("utenti");
            entity.HasKey(e => e.Id).HasName("utenti_pkey");

            entity.HasIndex(e => e.Username, "utenti_username_key").IsUnique();
            entity.HasIndex(e => e.Email, "utenti_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Username).HasMaxLength(50).HasColumnName("username");
            entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasMaxLength(255).HasColumnName("password_hash");
            entity.Property(e => e.Ruolo)
                .HasMaxLength(20)
                .HasDefaultValueSql("'user'::character varying")
                .HasColumnName("ruolo");
            entity.Property(e => e.DataCreazione)
                .HasDefaultValueSql("now()")
                .HasColumnName("data_creazione");
            entity.Property(e => e.DataUltimoLogin).HasColumnName("data_ultimo_login");
        });

        modelBuilder.Entity<Clienti>().ToTable("clienti");

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

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Licenzes)
                .HasForeignKey(e => e.IdCliente)
                .HasPrincipalKey(c => c.IdCliente);

            entity.HasOne(e => e.Impianto)
                .WithMany(i => i.Licenzes)
                .HasForeignKey(e => e.IdImpianto)
                .HasPrincipalKey(i => i.IdImpianto);
        });
    }
}
