using Entities;
using Entities.EntityType.Abstract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using Request = Entities.Models.Request;

namespace DataAccessLayer.DBConfig
{
    public class RequestConfig : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {

            // RequestId için benzersiz index oluşturuyoruz
            builder.HasIndex(r => r.RequestId)
                .IsUnique();  // RequestId'nin benzersiz olmasını sağlıyoruz

            // Talep ve TalepProcess arasındaki ilişkiyi kuruyoruz (Bir talep birden fazla sürece sahip olabilir)
            builder.HasMany(r => r.RequestProcess)  // Bir talep, birden fazla talep sürecine sahip olabilir
                .WithOne(rp => rp.Request)  // TalepProcess, Request ile ilişkilidir
                .HasForeignKey(rp => rp.RequestId)  // Foreign key olarak RequestId kullanılır
                .OnDelete(DeleteBehavior.Cascade);

            // Personel ve Request arasındaki ilişki (Bir personel birden fazla talep alabilir)
            builder.HasOne(r => r.Personel)  // Talep, Personel ile ilişkilidir
                   .WithMany(p => p.Requests)  // Bir personel, birden fazla talep alabilir
                   .HasForeignKey(r => r.PersonelId)  // Foreign key olarak PersonelId kullanılır
                   .OnDelete(DeleteBehavior.Restrict);  // Personel silindiğinde talepler korunur

            // Title için veri türü ve kısıtlamalar
            builder.Property(r => r.Title)
                   .IsRequired()  // Title alanı zorunlu
                   .HasMaxLength(200);  // Title alanı maksimum 200 karakter

            // Description için veri türü ve kısıtlamalar
            builder.Property(r => r.Description)
                   .HasMaxLength(1000);  // Description alanı maksimum 1000 karakter

            // CreatedDate için veri türü ve kısıtlamalar
            builder.Property(r => r.CreatedDate)
                   .IsRequired();  // CreatedDate alanı zorunlu

            // RequestStatus için veri türü ve kısıtlamalar
            builder.Property(r => r.RequestStatus)
                   .IsRequired();  // RequestStatus alanı zorunlu




            //builder.HasOne(r => r.Unit)
            //.WithMany(u => u.Request)
            //.HasForeignKey(r => r.UnitId)
            //.OnDelete(DeleteBehavior.NoAction); // No cascading delete

            //// Talep eden kullanıcı (Requester) ve Talep arasındaki ilişki
            //builder.HasOne(r => r.Requester)  // Talep, Requester ile ilişkilidir
            //       .WithMany(u => u.Requests)  // Bir kullanıcı, birden fazla talep oluşturabilir
            //       .HasForeignKey(r => r.RequesterId)  // Foreign key olarak RequesterId kullanılır
            //       .OnDelete(DeleteBehavior.Restrict);  // Kullanıcı silindiğinde talepler korunur

        }
    }
}
