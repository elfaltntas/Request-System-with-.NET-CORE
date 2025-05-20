using Entities.Dto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract.Authentication
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDto personelForRegistrationDto);
        Task<bool> ValidatePersonel(UserForAuthenticationDto personelForAuthDto);
        Task<TokenDto> CreateToken(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
        Task AddClaimToUser(string userId, string claimType, string claimValue);
        //Task GetUserRoles(UserForAuthenticationDto user);

    }
}
