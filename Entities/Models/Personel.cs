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
    public class Personel : AuditableEntity<string>, IEntity
    {
       public string PersonelId {  get; set; }
        //public string AssignedToPersonelId { get; set; } // Talebin atanacağı personel (örneğin, birimin yöneticisi)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    
        public string? UserId { get; set; } // User tablosuyla ilişki için Foreign Key
        public User User { get; set; } // İlişkili kullanıcı

        //public string? UnitId { get; set; } // User tablosuyla ilişki için Foreign Key
        //public Unit Unit { get; set; } // İlişkili kullanıcı
        public List<UnitPersonel> UnitPersonels { get; set; } = new List<UnitPersonel>(); // Many-to-many ilişkiyi yöneten ara tablo
        public ICollection<PersonelRoles> PersonelRoles { get; set; } // Ara tablo ilişkisi
        public List<Request> Requests { get; set; } = new List<Request>(); // Bir personel birden fazla talep ile ilişkili
       public List<RequestProcess> RequestProcess { get; set; } = new List<RequestProcess>(); // Bir personel birden fazla süreci kontrol edebilir.
        //public Personel()
        //{
        //    UnitPersonels = new List<UnitPersonel>(); // Birimler ile ilişkiler başlatılır
        //    Roles = new List<Roles>(); //Roller ile ilişkiler başlatılıyor 
        //}
    }

   
}
