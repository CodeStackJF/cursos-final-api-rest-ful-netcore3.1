using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cursos.Models
{
    public partial class CursosCTX : DbContext
    {
        public CursosCTX()
        {
        }

        public CursosCTX(DbContextOptions<CursosCTX> options)
            : base(options)
        {
            
        }

        public virtual DbSet<Curso> Curso { get; set; }
        public virtual DbSet<Estudiante> Estudiante { get; set; }
        public virtual DbSet<InscripcionCurso> InscripcionCurso { get; set; }
        public virtual DbSet<Periodo> Periodo { get; set; }
        public virtual DbSet<Usuarios> Usuarios {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Curso>(entity =>
            {
                entity.HasKey(e => e.IdCurso)
                    .HasName("PK__Curso__085F27D621F831CB");

                entity.Property(e => e.Codigo).IsUnicode(false);

                entity.Property(e => e.Descripcion).IsUnicode(false);
            });

            modelBuilder.Entity<Estudiante>(entity =>
            {
                entity.HasKey(e => e.IdEstudiante)
                    .HasName("PK__Estudian__B5007C246E883F92");

                entity.Property(e => e.Apellido).IsUnicode(false);

                entity.Property(e => e.Codigo).IsUnicode(false);

                entity.Property(e => e.Nombre).IsUnicode(false);

                entity.Property(e => e.NombreApellido)
                    .IsUnicode(false)
                    .HasComputedColumnSql("(concat([Nombre],' ',[Apellido]))");
            });

            modelBuilder.Entity<InscripcionCurso>(entity =>
            {
                entity.HasKey(e => new { e.IdEstudiante, e.IdPeriodo, e.IdCurso })
                    .HasName("PK__Inscripc__994C4A9CC8A4D1D5");

                entity.HasOne(d => d.Curso)
                    .WithMany(p => p.InscripcionCurso)
                    .HasForeignKey(d => d.IdCurso)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Inscripci__IdCur__4222D4EF");              
            });

            
            modelBuilder.Entity<Periodo>(entity =>
            {
                entity.HasKey(e => e.IdPeriodo)
                    .HasName("PK__Periodo__B44699FEC5F12AB8");

                entity.Property(e => e.IdPeriodo).ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
