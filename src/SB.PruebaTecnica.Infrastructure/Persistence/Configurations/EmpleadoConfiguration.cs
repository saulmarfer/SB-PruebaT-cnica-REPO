using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.PruebaTecnica.Domain.Entities;

namespace SB.PruebaTecnica.Infrastructure.Persistence.Configurations
{
    public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
    {
        public void Configure(EntityTypeBuilder<Empleado> builder)
        {
            builder.ToTable("Empleados");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.PrimerNombre).HasMaxLength(100);
            builder.Property(e => e.ApellidoPaterno).HasMaxLength(100).IsRequired();
            builder.Property(e => e.NumeroSeguroSocial).HasMaxLength(20).IsRequired();
            builder.Property(e => e.Departamento).HasMaxLength(100);

            builder.Property(e => e.FechaCreacion).IsRequired();

            builder.HasDiscriminator<int>("TipoEmpleado")
                .HasValue<EmpleadoAsalariado>(1)
                .HasValue<EmpleadoPorHoras>(2)
                .HasValue<EmpleadoPorComision>(3)
                .HasValue<EmpleadoAsalariadoPorComision>(4);
        }
    }
    public class EmpleadoAsalariadoConfiguration : IEntityTypeConfiguration<EmpleadoAsalariado>
    {
        public void Configure(EntityTypeBuilder<EmpleadoAsalariado> builder)
        {
            builder.Property(e => e.SalarioSemanal)
                .HasColumnName("SalarioSemanal")
                .HasColumnType("decimal(18,2)");
        }
    }

    public class EmpleadoPorHorasConfiguration : IEntityTypeConfiguration<EmpleadoPorHoras>
    {
        public void Configure(EntityTypeBuilder<EmpleadoPorHoras> builder)
        {
            builder.Property(e => e.SueldoPorHora)
                .HasColumnName("SueldoPorHora")
                .HasColumnType("decimal(18,2)");
            builder.Property(e => e.HorasTrabajadas)
                .HasColumnName("HorasTrabajadas")
                .HasColumnType("decimal(18,2)");
        }
    }

    public class EmpleadoPorComisionConfiguration : IEntityTypeConfiguration<EmpleadoPorComision>
    {
        public void Configure(EntityTypeBuilder<EmpleadoPorComision> builder)
        {
            builder.Property(e => e.VentasBrutas)
                .HasColumnName("VentasBrutas")
                .HasColumnType("decimal(18,2)");
            builder.Property(e => e.TarifaComision)
                .HasColumnName("TarifaComision")
                .HasColumnType("decimal(18,4)");
        }
    }

    public class EmpleadoAsalariadoPorComisionConfiguration : IEntityTypeConfiguration<EmpleadoAsalariadoPorComision>
    {
        public void Configure(EntityTypeBuilder<EmpleadoAsalariadoPorComision> builder)
        {
            // Mismas columnas físicas que EmpleadoPorComisionConfiguration: se
            // comparten a propósito (ver comentario de la clase de arriba).
            builder.Property(e => e.VentasBrutas)
                .HasColumnName("VentasBrutas")
                .HasColumnType("decimal(18,2)");
            builder.Property(e => e.TarifaComision)
                .HasColumnName("TarifaComision")
                .HasColumnType("decimal(18,4)");
            builder.Property(e => e.SalarioBase)
                .HasColumnName("SalarioBase")
                .HasColumnType("decimal(18,2)");
        }
    }
}