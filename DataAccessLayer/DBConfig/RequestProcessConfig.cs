using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.DBConfig
{
    public class RequestProcessConfig : IEntityTypeConfiguration<RequestProcess>
    {
        public void Configure(EntityTypeBuilder<RequestProcess> builder)
        {
           
            // Request ve RequestProcess arasındaki ilişki (Bir talep birden fazla sürece sahip olabilir)
            builder.HasOne(rp => rp.Request)  // RequestProcess, Request ile ilişkilidir
                   .WithMany(r => r.RequestProcess)  // Bir talep, birden fazla süreç ile ilişkilidir
                   .HasForeignKey(rp => rp.RequestId)  // Foreign key olarak RequestId kullanılır
                   .OnDelete(DeleteBehavior.Cascade);   // Talep silindiğinde ilgili süreçler de silinir

            //       .IsRequired();  // ProcessDate alanı zorunlu
            builder.HasOne(rp => rp.Unit)
                   .WithMany(u => u.RequestProcess)
                   .HasForeignKey(rp => rp.UnitId)
                   .OnDelete(DeleteBehavior.Restrict);
            // Status (RequestStatus enum) için veri türü ve kısıtlamalar
            builder.Property(rp => rp.Status)
                   .IsRequired();  // Status alanı zorunlu





            //builder.HasIndex(r => r.RequestProcessId)
            //    .IsUnique();
            //// ProcessedByUser ve RequestProcess arasındaki ilişki (Bir süreç bir kullanıcının işlemi olabilir)
            //builder.HasOne(rp => rp.ProcessedByUser)  // ProcessedByUser, RequestProcess ile ilişkilidir
            //       .WithMany()  // Bir kullanıcının birden fazla süreci olabilir
            //       .HasForeignKey(rp => rp.ProcessedBy);  // Foreign key olarak ProcessedBy kullanılır
            //       //.OnDelete(DeleteBehavior.Restrict);  // Kullanıcı silindiğinde süreçler korunur

            //// Personel ve RequestProcess arasındaki ilişki (Bir personel birden fazla süreci kontrol edebilir)
            //builder.HasOne(rp => rp.Personel)  // RequestProcess, Personel ile ilişkilidir
            //       .WithMany(p => p.RequestProcess)  // Bir personel, birden fazla süreçle ilişkilendirilebilir
            //       .HasForeignKey(rp => rp.PersonelId)  // Foreign key olarak PersonelId kullanılır
            //       .OnDelete(DeleteBehavior.Restrict);  // Personel silindiğinde süreçler korunur


            //// Notes alanı için veri türü ve kısıtlamalar
            //builder.Property(rp => rp.Notes)
            //       .HasMaxLength(500);  // Notes alanı maksimum 500 karakter olacak
        }
    }
}
