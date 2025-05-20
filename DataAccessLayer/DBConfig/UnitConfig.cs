using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace DataAccessLayer.DBConfig
{
    public class UnitConfig : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            // Id kolonunun otomatik olarak artan bir değer olduğunu belirtir.
            builder.HasKey(u => u.UnitId);

            // ParentUnitId sütununu nullable yapıyoruz çünkü üst birim olmayabilir.
            builder.Property(u => u.ParentUnitId)
                .IsRequired(false); // Nullable olarak ayarlandı.

            // Alt birimler için ilişkileri tanımlıyoruz (One-to-many).
            builder.HasMany(u => u.SubUnits)
                .WithOne(u => u.ParentUnit)
                .HasForeignKey(u => u.ParentUnitId);

            // Personel ile ilişkiler (Many-to-many).
            builder.HasMany(u => u.UnitPersonels)
                .WithOne(up => up.Unit)
                .HasForeignKey(up => up.Id);

            builder.HasMany(u => u.RequestProcess)
                   .WithOne(rp => rp.Unit)
                   .HasForeignKey(rp => rp.UnitId)
                   .OnDelete(DeleteBehavior.Restrict); // Cascade delete engellendi

            builder.Property(u => u.RoleId)
                   .IsRequired(); // RoleId zorunlu

            // Unit ile Role arasında Many-to-One ilişkiyi tanımlıyoruz
            builder.HasOne(u => u.Roles) // Her unit bir role'e sahip
                   .WithMany(r => r.Units) // Bir role birden fazla unit olabilir
                   .HasForeignKey(u => u.RoleId);  // RoleId dış anahtar olarak kullanılıyor
        }
    }
}
