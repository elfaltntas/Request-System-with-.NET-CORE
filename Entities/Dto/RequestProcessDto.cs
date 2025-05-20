using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class RequestProcessDto
    {
        public int RequestId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string ProcessDate { get; set; }
    }
}
