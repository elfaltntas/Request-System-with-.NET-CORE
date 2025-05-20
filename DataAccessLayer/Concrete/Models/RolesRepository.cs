using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Concrete.System;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.Models
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        // GetByIdAsync metodu
        
        public async Task AddAsync(Roles role)
        {
            await _context.Roles.AddAsync(role);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Roles> GetByRoleIdAsync(string RoleId)
        {
            // RoleManager ile RoleId'yi kullanarak rolü Identity API'sinden alıyoruz
            return await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == RoleId);
        }
        public async Task<IEnumerable<Roles>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        //public Roles GetByRoleId(int roleId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
