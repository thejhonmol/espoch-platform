using ESPOCH.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESPOCH.Infrastructure.Data;

public class ESPOCHDbContext : DbContext
{
    public ESPOCHDbContext(DbContextOptions<ESPOCHDbContext> options) : base(options)
    {
    }

    public DbSet<Rol> Roles { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Ubicacion> Ubicaciones { get; set; }
    public DbSet<Horario> Horarios { get; set; }
    public DbSet<Asistencia> Asistencias { get; set; }
    public DbSet<Ausencia> Ausencias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.IdRol);
            entity.Property(e => e.NombreRol).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(200);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.NombreCompleto).HasMaxLength(200).IsRequired();
            entity.Property(e => e.CorreoInstitucional).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ContrasenaHash).HasMaxLength(500);

            entity.HasOne(e => e.IdRolNavigation)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(e => e.IdRol)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.IdJefeDirectoNavigation)
                .WithMany(u => u.Colaboradores)
                .HasForeignKey(e => e.IdJefeDirecto)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.IdHorarioNavigation)
                .WithMany(h => h.Usuarios)
                .HasForeignKey(e => e.IdHorario)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.CorreoInstitucional).IsUnique();
        });

        modelBuilder.Entity<Ubicacion>(entity =>
        {
            entity.ToTable("Ubicaciones");
            entity.HasKey(e => e.IdUbicacion);
            entity.Property(e => e.CodigoUbicacion).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Direccion).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Horario>(entity =>
        {
            entity.ToTable("Horarios");
            entity.HasKey(e => e.IdHorario);
            entity.Property(e => e.NombreHorario).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Asistencia>(entity =>
        {
            entity.ToTable("Asistencias");
            entity.HasKey(e => e.IdAsistencia);
            entity.Property(e => e.Modalidad).HasMaxLength(20).IsRequired();
            entity.Property(e => e.EstadoPuntualidad).HasMaxLength(20).IsRequired();

            entity.HasOne(e => e.IdUsuarioNavigation)
                .WithMany(u => u.Asistencias)
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.IdUbicacionNavigation)
                .WithMany(u => u.Asistencias)
                .HasForeignKey(e => e.IdUbicacion)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ausencia>(entity =>
        {
            entity.ToTable("Ausencias");
            entity.HasKey(e => e.IdAusencia);
            entity.Property(e => e.Motivo).HasMaxLength(500).IsRequired();
            entity.Property(e => e.TipoAusencia).HasMaxLength(50).IsRequired();
            entity.Property(e => e.EstadoAprobacion).HasMaxLength(20).IsRequired();
            entity.Property(e => e.MotivoRechazo).HasMaxLength(500);

            entity.HasOne(e => e.IdUsuarioNavigation)
                .WithMany(u => u.AusenciasSolicitadas)
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.IdAprobadorNavigation)
                .WithMany(u => u.AusenciasAprobadas)
                .HasForeignKey(e => e.IdAprobador)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
