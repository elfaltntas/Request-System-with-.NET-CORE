using DataAccessLayer.Abstract.System;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
namespace DataAccessLayer.Abstract.Models
{
    public interface IUsersRepository : IGenericRepository<User>
    {
        Task<User> GetByIdAsync(string id);
        Task<IdentityResult> DeleteUserAsync(User user);

        // personel aktif pasif durumunun metodları
        User GetByUsername(string username);
        void Update(User user);
        //User GetByFullName(string firstName, string lastName);
    }
}
