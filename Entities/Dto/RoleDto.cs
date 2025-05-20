using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class RoleDto
    {
        public string RoleId { get; set; }
        public string Name { get; set; } // Görev başlığı
        public string Description { get; set; } // Görev açıklaması
    }
}
