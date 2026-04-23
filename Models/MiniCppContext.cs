using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerCPP.Models;

public partial class MiniCppContext : DbContext
{
    public MiniCppContext()
    {
    }

    public MiniCppContext(DbContextOptions<MiniCppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Clienti> Clientis { get; set; }

    public virtual DbSet<Commesse> Commesses { get; set; }

    public virtual DbSet<Impianti> Impiantis { get; set; }

    public virtual DbSet<Licenze> Licenzes { get; set; }

    public virtual DbSet<Utenti> Utentis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Mini-cpp;Username=postgres;Password=4everAL1");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Clienti>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("clienti_pkey");

            entity.ToTable("clienti");

            entity.HasIndex(e => e.Iva, "clienti_iva_key").IsUnique();

            entity.Property(e => e.IdCliente)
                .HasMaxLength(4)
                .HasColumnName("id_cliente");
            entity.Property(e => e.Iva)
                .HasMaxLength(11)
                .HasColumnName("iva");
            entity.Property(e => e.Nome)
                .HasMaxLength(255)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Commesse>(entity =>
        {
            entity.HasKey(e => e.IdCommessa).HasName("commesse_pkey");

            entity.ToTable("commesse");

            entity.Property(e => e.IdCommessa)
                .HasMaxLength(8)
                .HasColumnName("id_commessa");
        });

        modelBuilder.Entity<Impianti>(entity =>
        {
            entity.HasKey(e => e.IdImpianto).HasName("impianti_pkey");

            entity.ToTable("impianti");

            entity.Property(e => e.IdImpianto).HasColumnName("id_impianto");
            entity.Property(e => e.IdCliente)
                .HasMaxLength(4)
                .HasColumnName("id_cliente");
            entity.Property(e => e.IdCommessa)
                .HasMaxLength(8)
                .HasColumnName("id_commessa");
            entity.Property(e => e.Nome)
                .HasMaxLength(255)
                .HasColumnName("nome");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Impiantis)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("impianti_id_cliente_fkey");

            entity.HasOne(d => d.Commessa).WithMany(p => p.Impiantis)
                .HasForeignKey(d => d.IdCommessa)
                .HasConstraintName("impianti_id_commessa_fkey");
        });

        modelBuilder.Entity<Licenze>(entity =>
        {
            entity.HasKey(e => e.IdLicenza).HasName("licenze_pkey");

            entity.ToTable("licenze");

            entity.Property(e => e.IdLicenza)
                .HasMaxLength(32)
                .HasColumnName("id_licenza");
            entity.Property(e => e.DataAttivazione).HasColumnName("data_attivazione");
            entity.Property(e => e.DataScadenza).HasColumnName("data_scadenza");
            entity.Property(e => e.IdCliente)
                .HasMaxLength(4)
                .HasColumnName("id_cliente");
            entity.Property(e => e.IdImpianto).HasColumnName("id_impianto");
            entity.Property(e => e.Pagato).HasColumnName("pagato");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Licenzes)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("licenze_id_cliente_fkey");

            entity.HasOne(d => d.Impianto).WithMany(p => p.Licenzes)
                .HasForeignKey(d => d.IdImpianto)
                .HasConstraintName("licenze_id_impianto_fkey");
        });

        modelBuilder.Entity<Utenti>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("utenti_pkey");

            entity.ToTable("utenti");

            entity.HasIndex(e => e.Email, "ix_utenti_email");

            entity.HasIndex(e => e.Username, "ix_utenti_username");

            entity.HasIndex(e => e.Email, "utenti_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "utenti_username_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataCreazione)
                .HasDefaultValueSql("now()")
                .HasColumnName("data_creazione");
            entity.Property(e => e.DataUltimoLogin).HasColumnName("data_ultimo_login");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Ruolo)
                .HasMaxLength(20)
                .HasDefaultValueSql("'user'::character varying")
                .HasColumnName("ruolo");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
