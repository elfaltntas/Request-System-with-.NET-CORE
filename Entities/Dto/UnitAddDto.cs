using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class UnitAddDto
    {
        public string UnitId { get; set; }
        public string Name { get; set; }
        public string? ParentUnitId { get; set; }
        public string RoleId { get; set; }
    }
}
