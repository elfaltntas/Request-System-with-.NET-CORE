using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class RequestPersonelDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; } // Personelin adı
        public string LastName { get; set; }  // Personelin soyadı
    }
}
