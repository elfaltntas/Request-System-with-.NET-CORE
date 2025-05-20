using Entities.EntityType.Abstract;
using Entities.EntityType.Concrete;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class UnitPersonel : AuditableEntity<string>, IEntity
    {
        public int Id { get; set; }
        public string UnitId { get; set; }
        public Unit Unit { get; set; }
        public string PersonelId { get; set; }
        //public string PersonelName { get; set; }
        public Personel Personel { get; set; }
        public DateTime AssignedDate { get; set; } // Atama tarihi
    }
}
