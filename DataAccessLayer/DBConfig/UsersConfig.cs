using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.DBConfig
{
    public class UsersConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            //// Primary Key (EF Core otomatik olarak Id'yi primary key olarak kabul eder)
            builder.HasKey(u => u.Id);


            // Alanlar üzerinde kısıtlamalar
            builder.Property(u => u.FirstName)
                .IsRequired(); // Kullanıcı adı zorunlu
                

            builder.Property(u => u.Email)
                .IsRequired() // E-posta zorunlu
                .HasMaxLength(255); // Maksimum 255 karakter uzunluğu


            builder.HasOne(u => u.Personel)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.PersonelId)
               .IsRequired(false);


            //// İlişkiler
            //builder.HasMany(u => u.Requests) // Bir kullanıcı birden fazla talep oluşturabilir
            //    .WithOne(r => r.Requester) // Her talep bir kullanıcıya ait olmalıdır
            //    .HasForeignKey(r => r.RequesterId)
            //    .OnDelete(DeleteBehavior.Cascade); // Talep silindiğinde, ilişkili kullanıcıya zarar verilmez.

            //builder.HasMany(u => u.RequestProcess) // Bir kullanıcı birden fazla süreci işleyebilir
            //    .WithOne(rp => rp.ProcessedByUser) // Her süreç bir kullanıcı tarafından işlenir
            //    .HasForeignKey(rp => rp.ProcessedBy)
            //    .OnDelete(DeleteBehavior.Restrict); // Kullanıcı silindiğinde, sürecin silinmesi engellenir.

        }
    }
}
