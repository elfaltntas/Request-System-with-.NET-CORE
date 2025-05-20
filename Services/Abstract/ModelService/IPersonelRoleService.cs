using Entities.Dto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract.ModelService
{
    public interface IPersonelRoleService
    {
        //Task AssignRoleToPersonel(PersonelRoleDto personelRoleDto);
        Task<bool> AssignRoleToPersonel(PersonelRoleDto dto);
        //void AssignRoleToPersonel(PersonelRoleDto personelRoleDto);
    }
}
