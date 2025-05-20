using Entities.EntityType.Abstract;
using Entities.EntityType.Concrete;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class User : IdentityUser
    {
        public bool IsPersonel { get; set; }  // Personel olup olmadığını belirleyen özellik
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        //public string Email { get; set; } // Kullanıcı e-posta adresi
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        // User -> Personel birebir ilişkisi
        public string? PersonelId { get; set; }
        public Personel Personel { get; set; } // Eğer kullanıcı aynı zamanda bir personel ise ilişki kurulur
    }

}
