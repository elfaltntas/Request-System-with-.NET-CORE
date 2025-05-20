using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class PersonelWithUnitsDto
    {
        public string PersonelId { get; set; }
        public string FirstName { get; set; }  // First Name ekledik
        public string LastName { get; set; }   // Last Name ekledik
        public List<UnitDto> Units { get; set; }  // Birimleri tutacak liste
    }
}
