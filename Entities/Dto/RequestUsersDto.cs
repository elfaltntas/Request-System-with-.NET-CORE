using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    
        public class RequestUsersDto
        {
            public int RequestId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime CreatedDate { get; set; }
            public string PersonelName { get; set; } // Personelin adı
        }
    }


