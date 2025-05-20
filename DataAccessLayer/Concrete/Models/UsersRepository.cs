using Entities.Models;
using DataAccessLayer.Abstract.System;
using DataAccessLayer.Abstract.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DataAccessLayer.Concrete.System;
using Microsoft.AspNetCore.Identity;

namespace DataAccessLayer.Concrete.Models
{
    public class UsersRepository :  IUsersRepository
    {
        private readonly AppDbContext _context;
        public UsersRepository(AppDbContext context) 
        {
            _context = context;
        }

        public Task AddAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }


        public async Task<User> GetByIdAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return IdentityResult.Success; // Başarılı işlem için
        }


        //personelin aktif pasif kısmını kontrol ediyor
        public User GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == username);
        }


        public void Update(User user)
        {
            _context.Users.Update(user);
        }
        //public User GetByFullName(string firstName, string lastName)
        //{
        //    return _context.Users.FirstOrDefault(p => p.FirstName == firstName && p.LastName == lastName);
        //}

    }
}
