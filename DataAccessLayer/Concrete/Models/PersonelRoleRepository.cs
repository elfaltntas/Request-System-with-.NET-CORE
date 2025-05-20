using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Concrete.Repositories;
using DataAccessLayer.Concrete.System;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.Models
{
    public class PersonelRoleRepository : IPersonelRoleRepository
    {
        private readonly AppDbContext _context;
        public PersonelRoleRepository(AppDbContext context)  {
            _context = context;
        }
        public void Add(PersonelRoles personelRole)
        {
            _context.PersonelRoles.Add(personelRole); // PersonelRoles tablosuna ekleme
        }


        public bool HasRole(string personelId, string roleId)
        {
            return _context.PersonelRoles.Any(pr => pr.PersonelId == personelId && pr.RoleId == roleId);
        }

        // Değişiklikleri kaydetme metodu
        public void SaveChanges()
        {
            _context.SaveChanges(); // Değişiklikleri veritabanına kaydet
        }

        public IEnumerable<Personel> GetPersonelsByRoleId(string roleId)
        {
            return _context.Personels
                .Where(p => _context.PersonelRoles
                    .Any(pr => pr.PersonelId == p.PersonelId && pr.RoleId == roleId))
                .ToList();
        }

       
        public async Task<bool> IsPersonelExist(string personelId)
        {
            return await _context.Personels.AnyAsync(p => p.PersonelId == personelId);
        }

        public async Task<bool> IsRoleExist(string roleId)
        {
            return await _context.Roles.AnyAsync(r => r.RoleId == roleId);
        }

        public async Task AddPersonelRole(string personelId, string roleId)
        {
            var userRole = new IdentityUserRole<string>
            {
                UserId = personelId,
                RoleId = roleId
            };

            var personelRole = new PersonelRoles
            {
                PersonelId = personelId,
                RoleId = roleId
            };

            _context.UserRoles.Add(userRole);
            _context.PersonelRoles.Add(personelRole);

            await _context.SaveChangesAsync();
        }
    }
}

