using Entities;
using Entities.EntityType.Abstract;
using Entities.EntityType.Concrete;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Entities.Models
{ 
public class Unit 
{
 
    public string UnitId { get; set; }
    public string Name { get; set; } // Birimin adı

        public string RoleId { get; set; }
        public Roles Roles { get; set; }
        // Alt birimlerle ilişki (Many-to-one ilişkisi)
        public string? ParentUnitId { get; set; } // Üst birim ID'si
    public Unit? ParentUnit { get; set; } // Üst birim ile ilişki

        public string? SubUnitId { get; set; } // Üst birim ID'si
                                                  // Üst birime ait alt birimler (One-to-many ilişkisi)

        public List<Unit> SubUnits { get; set; } = new List<Unit>(); // Alt birimler
       
    public ICollection<RequestProcess> RequestProcess { get; set; } = new List<RequestProcess>(); // İlişkili talep süreçleri
    public List<UnitPersonel> UnitPersonels { get; set; } = new List<UnitPersonel>(); // Personel ile ilişkiler
    
    

}
}
