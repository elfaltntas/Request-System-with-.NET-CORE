using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class MoveUnitWithChildrenDto
    {
       
        public string UnitId { get; set; } // Taşınacak ana birim ID
        public string NewParentDepartmentId { get; set; } // Yeni departman ID
    }
}
