using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.EntityType.Concrete
{
    public class AuditableEntity<T> : EntityBase<T>
    {
        public virtual DateTime? HireDate { get; set; } // İşe başlama tarihi
        public virtual DateTime? TerminationDate { get; set; } // İşten çıkış tarihi
        public virtual DateTime? ActiveDate { get; set; } // Aktif olduğu tarih
        public virtual DateTime? DeactivationDate { get; set; }  // Pasif olduğu tarih
        public virtual bool IsDeleted { get; set; } = false;
        public virtual bool IsActive { get; set; } = true;




        //public virtual string? Detail { get; set; }
        //public virtual DateTime CreatedDate { get; set; } = DateTime.Now; // override CreatedDate = new DateTime(2020/01/01);
        //public virtual DateTime ModifiedDate { get; set; } = DateTime.Now;

        //public virtual int CreatedByUserId { get; set; } = 1;
        //public virtual int ModifiedByUserId { get; set; } = 1;
    }

}
