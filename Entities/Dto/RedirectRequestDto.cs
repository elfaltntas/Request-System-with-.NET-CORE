using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class RedirectRequestDto
    {
        public string RequestId { get; set; }
        public string NewUnitId { get; set; }
        //public string? TargetUnitId { get; set; }
        public string? TargetPersonelId { get; set; }
        public RequestStatus NewStatus { get; set; } // Enum tipinde yeni durum
        public string Notes { get; set; }
    }

}
