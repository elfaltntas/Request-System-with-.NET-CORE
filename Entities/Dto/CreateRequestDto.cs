using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class CreateRequestDto
    {
        public int UserId { get; set; } // Talep eden kullanıcının ID'si
        public int PersonelId { get; set; } // Personel ID'si
        public string Title { get; set; } // Talep başlığı
        public string Description { get; set; } // Talep açıklaması
        public string UnitId { get; set; }  
    }
}

