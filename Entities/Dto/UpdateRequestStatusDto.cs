using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class UpdateRequestStatusDto
    {
        public string RequestId { get; set; }
        public RequestStatus NewStatus { get; set; }
        public string Notes { get; set; }
    }

}
