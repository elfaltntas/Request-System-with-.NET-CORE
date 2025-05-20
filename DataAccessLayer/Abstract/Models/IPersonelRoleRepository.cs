using DataAccessLayer.Abstract.System;
using DataAccessLayer.Concrete.Repositories;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.Models
{
    public interface IPersonelRoleRepository 
    {
        void Add(PersonelRoles personelRole); // PersonelRol ekleme metodu
        void SaveChanges(); // Değişiklikleri kaydetme metodu
        bool HasRole(string personelId, string roleId);
        IEnumerable<Personel> GetPersonelsByRoleId(string roleId);

        Task<bool> IsPersonelExist(string personelId);
        Task<bool> IsRoleExist(string roleId);
        Task AddPersonelRole(string personelId, string roleId);
        // Role ekleme ve özel sorgular için ek metodlar tanımlanabilir
    }
}
