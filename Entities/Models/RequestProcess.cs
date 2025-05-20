using Entities.EntityType.Abstract;
using Entities.EntityType.Concrete;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class RequestProcess
        //: AuditableEntity<int>, IEntity
    {
        public int Id { get; set; }
        //public string RequestProcessId { get; set; } 
        public string RequestId { get; set; } // Hangi talep ile ilişkilendirildiği
        public Request Request { get; set; } // İlişkili talep
        //public DateTime ProcessDate { get; set; } // Sürecin gerçekleştiği tarih
        public RequestStatus Status { get; set; } // Sürecin durumu (Pending, InProgress, etc.)
        public DateTime? CreatedDate { get; set; } = DateTime.Now; // override CreatedDate = new DateTime(2020/01/01);
        public  DateTime? ModifiedDate { get; set; } = DateTime.Now;
        public virtual bool IsDeleted { get; set; } = false;
        public virtual bool IsActive { get; set; } = true;
        public string? Notes { get; set; } // Süreçle ilgili notlar veya açıklamalar
        public string UnitId { get; set; }
        public Unit Unit { get; set; }
        public string? PersonelId { get; set; }
        public Personel Personel { get; set; }

    }

}
