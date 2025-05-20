using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class UnitTreeDto
    {
        public string UnitName { get; set; }
        public IEnumerable<UnitTreeDto> Children { get; set; } = new List<UnitTreeDto>();
    }

}
