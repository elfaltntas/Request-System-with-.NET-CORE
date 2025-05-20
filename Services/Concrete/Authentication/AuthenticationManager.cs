using AutoMapper;
using Entities.Dto;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Services.Abstract.Authentication;
using Services.Abstract.ModelService;
using Services.Abstract.Systems;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete.Authentication
{
    public class AuthenticationManager : IAuthenticationService
    {
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private User _user;
        private readonly IPersonelService _personelService;

        public AuthenticationManager(ILoggerService logger,
            IMapper mapper,
            UserManager<User> userManager,
            IConfiguration configuration,
            IPersonelService personelService)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _personelService = personelService;
        }

               //kullanıcı kaydı
        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto)
        {
            // Null kontrolleri
            if (userForRegistrationDto == null)
            {
                throw new ArgumentNullException(nameof(userForRegistrationDto), "Kullanıcı verisi boş olamaz.");
            }

            if (string.IsNullOrEmpty(userForRegistrationDto.UserName))
            {
                throw new ArgumentNullException(nameof(userForRegistrationDto.UserName), "Kullanıcı adı boş olamaz.");
            }

            if (string.IsNullOrEmpty(userForRegistrationDto.Password))
            {
                throw new ArgumentNullException(nameof(userForRegistrationDto.Password), "Şifre boş olamaz.");
            }

            // DTO'dan User nesnesini oluştur
            var user = _mapper.Map<User>(userForRegistrationDto);

            // User tablosuna ekleme
            var result = await _userManager.CreateAsync(user, userForRegistrationDto.Password);
            if (!result.Succeeded)
            {
                return result; // Eğer kullanıcı eklenemediyse, hatayı döndür
            }

            // Kullanıcı kaydı başarılı olduğunda, Personel kaydı yap
            var personel = new Personel
            {
                PersonelId = user.Id,
                UserId= user.Id,// PersonelId, UserId ile aynı olacak
                FirstName = userForRegistrationDto.FirstName,    // Personel adı
                LastName = userForRegistrationDto.LastName, // Personel soyadı
                Username = userForRegistrationDto.UserName,  // Personel kullanıcı adı
                Password = userForRegistrationDto.Password,  // Şifre
                Email = userForRegistrationDto.Email  // Email
            };


            // Personel kaydını ekleyin
            await _personelService.AddPersonelAsync(personel);

            // Veritabanına değişiklikleri kaydedin (bu adımı `_personelService` içinde yapabilirsiniz)
            await _personelService.SaveChangesAsync();

            return result; // Başarılı sonucu döndür
        }


        // Kullanıcı doğrulama
        public async Task<bool> ValidatePersonel(UserForAuthenticationDto userForAuthenticationDto)
        {
            _user = await _userManager.FindByNameAsync(userForAuthenticationDto.UserName);
            if (_user == null)
            {
                _logger.LogWarning($"{nameof(ValidatePersonel)}: User {userForAuthenticationDto.UserName} not found.");
                return false; // Kullanıcı bulunamadıysa false döndür
            }

            var result = await _userManager.CheckPasswordAsync(_user, userForAuthenticationDto.Password);
            if (!result)
            {
                _logger.LogWarning($"{nameof(ValidatePersonel)}: Authentication failed for user {userForAuthenticationDto.UserName}. Wrong password.");
                return false; // Parola yanlışsa false döndür
            }

            // Kullanıcı giriş yaptıysa, AspNetUserLogins tablosuna kaydet
            var loginInfo = new UserLoginInfo("JWT", _user.Id, "JWT Login");
            try
            {
                await _userManager.AddLoginAsync(_user, loginInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ValidatePersonel)}: Error adding login info for user {userForAuthenticationDto.UserName}: {ex.Message}");
                return false; // Login kaydı sırasında hata olursa false döndür
            }

            // Rol kontrolü
            //var roles = await _userManager.GetRolesAsync(_user);
            //_logger.LogInformation($"User roles: {string.Join(", ", roles)}"); // Kullanıcının rollerini logla
            //if (!roles.Contains("admin"))
            //{
            //    _logger.LogWarning($"{nameof(ValidatePersonel)}: User does not have the required role.");
            //    return false; // Eğer admin rolü yoksa, false döndür
            //}

            return true; // Eğer kullanıcı uygun role sahipse, true döndür
        }



        private SigningCredentials GetSignInCredentials()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["secretKey"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, _user.UserName)
    };

            var roles = await _userManager.GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
        public async Task AddClaimToUser(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var claim = new Claim(claimType, claimValue);
                var result = await _userManager.AddClaimAsync(user, claim);

                if (!result.Succeeded)
                {
                    throw new Exception($"Error adding claim: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                throw new Exception("User not found.");
            }
        }


        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signinCredentials,
            List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken(
                    issuer: jwtSettings["validIssuer"],
                    audience: jwtSettings["validAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                    signingCredentials: signinCredentials);

            return tokenOptions;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                RoleClaimType = ClaimTypes.Role // Rol claim türü olarak ClaimTypes.Role kullanıyoruz
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token.");
            }
            return principal;
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user is null ||
                user.RefreshToken != tokenDto.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new RefreshTokenBadRequestException();

            _user = user;
            return await CreateToken(populateExp: false);
        }



        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            var signinCredentials = GetSignInCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signinCredentials, claims);

            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken = refreshToken;

            if (populateExp)
                _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(_user);

            // AspNetUserTokens tablosuna kaydet
            var tokenEntry = await _userManager.GetAuthenticationTokenAsync(_user, "JWT", "AccessToken");
            if (tokenEntry == null)
            {
                await _userManager.SetAuthenticationTokenAsync(_user, "JWT", "AccessToken", new JwtSecurityTokenHandler().WriteToken(tokenOptions));
            }
            else
            {
                await _userManager.RemoveAuthenticationTokenAsync(_user, "JWT", "AccessToken");
                await _userManager.SetAuthenticationTokenAsync(_user, "JWT", "AccessToken", new JwtSecurityTokenHandler().WriteToken(tokenOptions));
            }

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }


        //public Task<bool> ValidatePersonel(PersonelForAuthenticationDto personelForAuthDto)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
