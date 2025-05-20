using Entities.Models; // Personel sınıfını içeri alıyoruz
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.DBConfig
{
    public class PersonelConfig : IEntityTypeConfiguration<Personel>
    {
        public void Configure(EntityTypeBuilder<Personel> builder)
        {
            // Id kolonunu belirtiyoruz
            builder.HasKey(p => p.PersonelId);
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100); // Ad alanı zorunlu

            // User -> Personel birebir ilişki
            builder.HasOne(p => p.User)
                   .WithOne(u => u.Personel)
                   .HasForeignKey<Personel>(p => p.UserId)
                   .IsRequired(false);
            //.OnDelete(DeleteBehavior.Cascade); // User silinirse ilgili Personel silinir

            // UnitPersonels tablosuyla ilişkileri tanımlıyoruz (Many-to-many).
            builder.HasMany(p => p.UnitPersonels)
                .WithOne(up => up.Personel) // Ara tablonun Personel ile ilişkisinin olduğu taraf
                .HasForeignKey(up => up.PersonelId) // UnitPersonel tablosundaki PersonelId yabancı anahtar
                .OnDelete(DeleteBehavior.Restrict); // Silme işlemi kısıtlanabilir, çünkü bir personel silindiğinde birimler etkilenmesin.

            builder.HasMany(p => p.Requests)
                   .WithOne(r => r.Personel)  // Her talep bir personel'e ait olacak
                   .HasForeignKey(r => r.PersonelId); // PersonelId, Request tablosunda foreign key olacak






            //// Roles tablosu ile ilişkileri tanımlıyoruz (Many-to-many).
            //builder.HasMany(p => p.Roles) // Bir Personel'in birden fazla rolü olabilir
            // .WithOne(r => r.Personel) // Her Role bir Personel'e aittir
            // .HasForeignKey(r => r.PersonelId) // PersonelId, Roles tablosunda foreign key
            // .OnDelete(DeleteBehavior.Restrict); // Personel silindiğinde roller silinmesin, kısıtla


            //// Personel ve RequestProcess arasındaki one-to-many ilişki (Bir personel birden fazla süreç ile ilişkili olabilir)
            //builder.HasMany(p => p.RequestProcess)
            //       .WithOne(rp => rp.Personel)  // Her süreç bir personel'e ait olacak
            //       .HasForeignKey(rp => rp.PersonelId)  // PersonelId, RequestProcess tablosunda foreign key olacak
            //       .OnDelete(DeleteBehavior.Restrict);  // Silme işlemi sırasında referansın korunmasını sağlar
        }
    }
}
