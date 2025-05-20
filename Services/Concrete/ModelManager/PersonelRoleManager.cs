using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Concrete.Models;
using DataAccessLayer.Concrete.System;
using Entities.Dto;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Abstract.Authentication;
using Services.Abstract.ModelService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete.ModelManager
{
    public class PersonelRoleManager : IPersonelRoleService
    {
        private readonly IPersonelRoleRepository _personelRoleRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly UserManager<User> _userManager;

        public PersonelRoleManager(
            IPersonelRoleRepository personelRoleRepository,
            IAuthenticationService authenticationService,
            UserManager<User> userManager)
        {
            _personelRoleRepository = personelRoleRepository;
            _authenticationService = authenticationService;
            _userManager = userManager;
        }

        //personel ataması yapar ve claim ekler
        public async Task<bool> AssignRoleToPersonel(PersonelRoleDto dto)
        {
            var personelExists = await _personelRoleRepository.IsPersonelExist(dto.Id);
            var roleExists = await _personelRoleRepository.IsRoleExist(dto.RoleId);

            if (!personelExists || !roleExists)
                return false;

            // Rol atamasını gerçekleştir
            await _personelRoleRepository.AddPersonelRole(dto.Id, dto.RoleId);

            // Kullanıcıyı bul
            var user = await _userManager.FindByIdAsync(dto.Id.ToString());
            if (user != null)
            {
                // Claim'i ekle
                await _authenticationService.AddClaimToUser(dto.Id.ToString(), ClaimTypes.Role, dto.RoleId.ToString());
            }

            return true;
        }
    }


}

