using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace DataAccessLayer.DBConfig
{
    public class RolesConfig : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            // Primary Key olarak Id'yi tanımlıyoruz
            builder.HasKey(r => r.RoleId);
            builder.Property(r => r.RoleName).IsRequired().HasMaxLength(50); // Rol adı zorunlu
            builder.Property(r => r.RoleId)
            .ValueGeneratedOnAdd(); // Auto-increment
                                    //// Status property'sini zorunlu yapıyoruz
                                    //builder.Property(r => r.Status)
                                    //    .IsRequired();
            builder.HasMany(r => r.Units)
                              .WithOne(u => u.Roles)
                              .HasForeignKey(u => u.RoleId);  // RoleId dış anahtar olarak kullanılıyor






            //// Personel ile olan ilişkiyi tanımlıyoruz (Many-to-One ilişkisi)
            //builder.HasOne(r => r.Personel) // Her Role bir Personel'e aittir
            //    .WithMany(p => p.Roles) // Bir Personel birden fazla Role'e sahip olabilir
            //    .HasForeignKey(r => r.PersonelId);// PersonelId foreign key olarak kullanılacak

        }
    }
}
