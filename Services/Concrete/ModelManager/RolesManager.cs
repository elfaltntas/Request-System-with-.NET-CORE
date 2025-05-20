using DataAccessLayer.Abstract.Models;
using Entities.Dto;
using Entities.Models;
using Microsoft.AspNetCore.Identity;  // RoleManager için gerekli namespace
using Microsoft.EntityFrameworkCore;
using Services.Abstract.ModelService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete.ModelManager
{
    public class RolesManager : IRolesService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly RoleManager<IdentityRole> _roleManager;  // RoleManager ekledim

        public RolesManager(IRoleRepository roleRepository, RoleManager<IdentityRole> roleManager)
        {
            _roleRepository = roleRepository;
            _roleManager = roleManager;  // RoleManager'ı enjekte ettim
        }

        //rol ve roleclaim ekler
        public async Task AddRoleAsync(RoleDto roleDto)
        {
            if (string.IsNullOrWhiteSpace(roleDto.Name))
            {
                throw new ArgumentException("Rol başlığı boş olamaz.");
            }

            // AspNetRoles tablosunda bu rol var mı diye kontrol et
            var existingRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleDto.Name);
            if (existingRole != null)
            {
                throw new InvalidOperationException($"Rol {roleDto.Name} zaten mevcut.");
            }

            // Yeni rolü AspNetRoles tablosuna ekle
            var role = new IdentityRole(roleDto.Name); // IdentityRole tipi kullanıyoruz
            var result = await _roleManager.CreateAsync(role); // RoleManager ile ekliyoruz

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Rol oluşturulamadı: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // AspNetRoles tablosundaki ID'yi al
            var createdRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleDto.Name);
            if (createdRole == null)
            {
                throw new InvalidOperationException("Oluşturulan rol bulunamadı.");
            }

            // Role ekleme işlemi başarılı, şimdi AspNetRoleClaims tablosuna ekleme yapalım
            var claimResult = await _roleManager.AddClaimAsync(createdRole, new Claim("Role", roleDto.Name));
            if (!claimResult.Succeeded)
            {
                throw new InvalidOperationException($"Rol claim'i eklenemedi: {string.Join(", ", claimResult.Errors.Select(e => e.Description))}");
            }

            // Kendi repository'inize ekleyin
            var roleEntity = new Roles
            {
                RoleId = createdRole.Id, // AspNetRoles'tan alınan ID
                RoleName = roleDto.Name,
                Description = roleDto.Description
            };

            await _roleRepository.AddAsync(roleEntity);
            await _roleRepository.SaveChangesAsync();
        }
        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();

            // Manual Mapping: Role -> RoleDto dönüşümü
            var roleDtos = new List<RoleDto>();
            foreach (var role in roles)
            {
                roleDtos.Add(new RoleDto
                {
                    RoleId = role.RoleId,
                    Name = role.RoleName,
                    Description = role.Description
                });
            }

            return roleDtos;
        }
    }
    //public async Task AddRoleAsync(RoleDto roleDto)
    //{
    //    if (string.IsNullOrWhiteSpace(roleDto.Name))
    //    {
    //        throw new ArgumentException("Rol başlığı boş olamaz.");
    //    }

    //    // AspNetRoles tablosunda bu rol var mı diye kontrol et
    //    var existingRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleDto.Name);
    //    if (existingRole != null)
    //    {
    //        throw new InvalidOperationException($"Rol {roleDto.Name} zaten mevcut.");
    //    }

    //    // Yeni role'ü AspNetRoles tablosuna ekle
    //    var role = new IdentityRole(roleDto.Name); // IdentityRole tipi kullanıyoruz
    //    var result = await _roleManager.CreateAsync(role); // RoleManager ile ekliyoruz

    //    if (!result.Succeeded)
    //    {
    //        throw new InvalidOperationException($"Rol oluşturulamadı: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    //    }

    //    // AspNetRoles tablosundaki ID'yi al
    //    var createdRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleDto.Name);
    //    if (createdRole == null)
    //    {
    //        throw new InvalidOperationException("Oluşturulan rol bulunamadı.");
    //    }


    //    // Kendi repository'inize ekleyin
    //    var roleEntity = new Roles
    //    {
    //        RoleId = createdRole.Id, // AspNetRoles'tan alınan ID
    //        RoleName = roleDto.Name,
    //        Description = roleDto.Description
    //    };

    //    await _roleRepository.AddAsync(roleEntity);
    //    await _roleRepository.SaveChangesAsync();
    //}


}
