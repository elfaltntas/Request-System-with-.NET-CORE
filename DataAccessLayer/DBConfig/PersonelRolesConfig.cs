using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Configurations
{
    public class PersonelRolesConfig : IEntityTypeConfiguration<PersonelRoles>
    {
        public void Configure(EntityTypeBuilder<PersonelRoles> builder)
        {
            builder.HasKey(pr => new { pr.PersonelId, pr.RoleId }); // Composite Key

            builder.HasOne(pr => pr.Personel)
                   .WithMany(p => p.PersonelRoles)
                   .HasForeignKey(pr => pr.PersonelId)
                   .OnDelete(DeleteBehavior.Cascade); // İlişki türü

            builder.HasOne(pr => pr.Role)
                   .WithMany(r => r.PersonelRoles)
                   .HasForeignKey(pr => pr.RoleId)
                   .OnDelete(DeleteBehavior.Cascade); // İlişki türü
        }
    }
}
