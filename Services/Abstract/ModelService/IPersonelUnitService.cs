using Entities.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract.ModelService
{
    public interface IPersonelUnitService
    {
        void AssignUnitToPersonel(PersonelUnitDto personelUnitDto);
    }
}
