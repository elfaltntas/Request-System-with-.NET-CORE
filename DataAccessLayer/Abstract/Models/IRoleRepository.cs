using DataAccessLayer.Abstract.System;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.Models
{
    public interface IRoleRepository 
    {
        // Role'lere özel metotlar
        //Roles GetByRoleId(int roleId); // Role ID ile role getir
        Task AddAsync(Roles role);
        Task SaveChangesAsync();
        Task<Roles> GetByRoleIdAsync(string RoleId);
        Task<IEnumerable<Roles>> GetAllRolesAsync();
    }
}
