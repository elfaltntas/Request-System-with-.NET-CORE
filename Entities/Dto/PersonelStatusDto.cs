using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class PersonelStatusDto
    {
        public string FullName { get; set; } // Ad ve Soyad

        public DateTime? HireDate { get; set; } // İşe başlama tarihi
        public DateTime? TerminationDate { get; set; } // İşten çıkış tarihi
        public DateTime? ActiveDate { get; set; } // Aktif olduğu tarih
        public DateTime? DeactivationDate { get; set; } // Pasif olduğu tarih
        public DateTime? AssignedDate { get; set; } // Birime atama tarihi
        public bool IsActive { get; set; } // Aktiflik durumu
    }

}
