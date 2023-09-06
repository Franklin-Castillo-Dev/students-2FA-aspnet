using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MiParcial.Models;

public partial class Cc101020Context : DbContext
{
    public Cc101020Context()
    {
    }

    public Cc101020Context(DbContextOptions<Cc101020Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Curso> Cursos { get; set; }

    public virtual DbSet<Estudiante> Estudiantes { get; set; }

    public virtual DbSet<Inscripcione> Inscripciones { get; set; }

    public virtual DbSet<Profesore> Profesores { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //    => optionsBuilder.UseSqlServer("server=DESKTOP-NT0UKG7\\SQLEXPRESS; database=CC101020; integrated security = true;TrustServerCertificate=True;");
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Curso>(entity =>
        {
            entity.HasKey(e => e.CursoId).HasName("PK__Cursos__7E023A37979404EF");

            entity.Property(e => e.CursoId).HasColumnName("CursoID");
            entity.Property(e => e.NombreCurso)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProfesorId).HasColumnName("ProfesorID");

            entity.HasOne(d => d.Profesor).WithMany(p => p.Cursos)
                .HasForeignKey(d => d.ProfesorId)
                .HasConstraintName("FK__Cursos__Profesor__412EB0B6");
        });

        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.HasKey(e => e.EstudianteId).HasName("PK__Estudian__6F768338B31148CB");

            entity.Property(e => e.EstudianteId).HasColumnName("EstudianteID");
            entity.Property(e => e.FechaNacimiento).HasColumnType("date");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Estudiantes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Estudiant__UserI__4222D4EF");
        });

        modelBuilder.Entity<Inscripcione>(entity =>
        {
            entity.HasKey(e => e.InscripcionId).HasName("PK__Inscripc__16831699FE588FC8");

            entity.Property(e => e.InscripcionId).HasColumnName("InscripcionID");
            entity.Property(e => e.CursoId).HasColumnName("CursoID");
            entity.Property(e => e.EstudianteId).HasColumnName("EstudianteID");

            entity.HasOne(d => d.Curso).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.CursoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inscripci__Curso__4316F928");

            entity.HasOne(d => d.Estudiante).WithMany(p => p.Inscripciones)
                .HasForeignKey(d => d.EstudianteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inscripci__Estud__440B1D61");
        });

        modelBuilder.Entity<Profesore>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Codigo2Fa)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Codigo2FA");
            entity.Property(e => e.CodigoVerificacion)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CodigoVerificacion");
            entity.Property(e => e.CreacionCodigo2Fa)
                .HasColumnType("datetime")
                .HasColumnName("CreacionCodigo2FA");
            entity.Property(e => e.CreacionCodigoVerificacion)
                .HasColumnType("datetime")
                .HasColumnName("CreacionCodigoVerificacion");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Habilitar2Fa).HasColumnName("Habilitar2FA");
            entity.Property(e => e.Password)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.Permiso)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TokenVerificacionCorreo)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.UltimoAcceso).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
