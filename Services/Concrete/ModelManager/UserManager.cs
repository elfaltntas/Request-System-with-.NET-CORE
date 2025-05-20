using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Concrete.Models;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Services.Abstract.ModelService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete.ModelManager
{
    public class UserManager : IUserService
    {
        private readonly UserManager<User> _userManager;

        private readonly IUnitPersonelRepository _unitPersonelRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPersonelRepository _personelRepository;

        public UserManager(UserManager<User> userManager, IUnitPersonelRepository unitPersonelRepository, IHttpContextAccessor httpContextAccessor, IPersonelRepository personelRepository)
        {
            _userManager = userManager;
            _unitPersonelRepository = unitPersonelRepository;
            _httpContextAccessor = httpContextAccessor;
            _personelRepository = personelRepository;
        }
       
        //public string DeleteUser(string Id)
        //{
        //    string result;
        //    var user = _userManager.FindByIdAsync(Id).Result;

        //    if (user != null)
        //    {
        //        var deleteResult = _userManager.DeleteAsync(user).Result;

        //        if (deleteResult.Succeeded)
        //        {
        //            result = "User Deleted";
        //        }
        //        else
        //        {
        //            result = "Error while deleting user.";
        //        }
        //    }
        //    else
        //    {
        //        result = "User Not Found";
        //    }

        //    return result;
        //}

    }
}

//public string DeleteUser(string userId)
//{
//    string result;

//    // Kullanıcıyı User tablosundan al
//    var user = _userManager.FindByIdAsync(userId).Result;
//    if (user == null)
//    {
//        return "User Not Found";
//    }

//    // Personel kaydını al
//    var unitPersonels = _unitPersonelRepository.GetByPersonelId(userId);
//    if (unitPersonels != null && unitPersonels.Any())
//    {
//        // Personel kaydını sil
//        foreach (var unitPersonel in unitPersonels)
//        {
//            _unitPersonelRepository.Delete(unitPersonel); // UnitPersonel kaydını sil
//        }
//        _unitPersonelRepository.SaveChangesAsync();
//    }

//    // Personel kaydını sil
//    var personel = _personelRepository.GetById(userId);
//    if (personel != null)
//    {
//        _unitPersonelRepository.DeletePersonel(personel); // Personel kaydını sil
//        _unitPersonelRepository.SaveChangesAsync();
//    }

//    // Kullanıcıyı User tablosundan sil
//    try
//    {
//        var deleteResult = _userManager.DeleteAsync(user).Result;
//        if (deleteResult.Succeeded)
//        {
//            result = "User and related Personel and UnitPersonel deleted successfully.";
//        }
//        else
//        {
//            result = "Error while deleting user.";
//        }
//    }
//    catch (Exception ex)
//    {
//        result = $"Error while deleting user: {ex.Message}";
//    }

//    return result;
//}



//public string FindById(string Id)
//{
//    var user = _userManager.FindByIdAsync(Id).Result;

//    return user != null ? user.UserName : "User Not Found";
//}


////kullanıcı logini kontrol ediyor
//public bool IsUserLoggedIn()
//{
//    var user = _httpContextAccessor.HttpContext?.User;

//    // Kullanıcının kimlik doğrulaması yapılmış mı?
//    return user != null && user.Identity?.IsAuthenticated == true;
//}

//public string GetLoggedInUserId()
//{
//    var user = _httpContextAccessor.HttpContext?.User;

//    if (user?.Identity?.IsAuthenticated ?? false)
//    {
//        // Kullanıcının ID'sini Claim'den alıyoruz
//        return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//    }
//    return null;
//}

