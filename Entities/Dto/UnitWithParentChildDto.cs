using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class UnitWithParentChildDto
    {
        public string UnitName { get; set; }
        public List<string> ParentUnits { get; set; }
        public List<string> SubUnits { get; set; }
    }
}
