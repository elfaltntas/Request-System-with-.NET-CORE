using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class PersonelUnitDto
    {
        public string PersonelId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UnitId { get; set; }
        //public string ? NewRole  { get; set; } // Yeni rol parametresi
    }
}
