using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class FindUnitDto
    {
        public string  UnitId { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; } // Üst birim adı
    }
}
