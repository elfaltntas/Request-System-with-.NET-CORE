using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace DataAccessLayer.DBConfig
{
    public class UnitPersonelConfig : IEntityTypeConfiguration<UnitPersonel>
    {
        public void Configure(EntityTypeBuilder<UnitPersonel> builder)
        {
            //    // Bileşik Anahtar (Composite Key) kullanarak UnitId ve PersonelId'yi anahtar olarak belirleyin
            //    builder.HasKey(up => new { up.Id, up.PersonelId });
            builder.HasKey(up => new { up.UnitId, up.PersonelId });
            // Unit ile olan ilişkisini tanımlıyoruz (Many-to-One)
            builder.HasOne(up => up.Unit)
                   .WithMany(u => u.UnitPersonels) // Unit'in birden fazla UnitPersonel'i olabilir
                   .HasForeignKey(up => up.UnitId); // UnitId'nin dış anahtar olarak kullanılması

            // Personel ile olan ilişkisini tanımlıyoruz (Many-to-One)
            builder.HasOne(up => up.Personel)
                   .WithMany(p => p.UnitPersonels) // Personel'in birden fazla UnitPersonel'i olabilir
                   .HasForeignKey(up => up.PersonelId); // PersonelId'nin dış anahtar olarak kullanılması


           

            
                //builder.HasOne(up => up.Unit)
                //.WithMany(u => u.UnitPersonels)
                //.HasForeignKey(up => up.UnitId);

            //// PersonelName alanı, burada sadece değer olarak tutuluyor.
            //// Genellikle ek özellikler ya da ilişkiler üzerinden sorgular yapılır.
            //builder.Property(up => up.PersonelName)
            //       .IsRequired();// Bu alanın zorunlu olmasını belirtiyoruz, eğer böyle bir durum istiyorsanız



        }
    }
}
