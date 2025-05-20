using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Concrete.System;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.Models
{
    public class PersonelRepository : IPersonelRepository
    {
        private readonly AppDbContext _context;
        public PersonelRepository(AppDbContext context)
        {
            _context = context;
        }
        public async  Task AddAsync(Personel personel)
        {
            await _context.Personels.AddAsync(personel);
        }


        public Personel GetByFullName(string firstName, string lastName)
        {
            return _context.Personels.FirstOrDefault(p => p.FirstName == firstName && p.LastName == lastName);
        }

        public Personel GetById(string personelId)
        {
            return _context.Personels.FirstOrDefault(p => p.PersonelId == personelId);
        }
    
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Personel> GetAll(bool onlyActive = false)
        {
            var query = _context.Personels
                .Include(p => p.UnitPersonels)
                .ThenInclude(up => up.Unit) // Birimleri de dahil et
                .AsQueryable();

            if (onlyActive)
                query = query.Where(p => p.IsActive);

            return query.ToList();
        }


        public void Update(Personel personel)
        {
            _context.Personels.Update(personel);
        }

        public async Task<Personel> GetByIdAsync(string personelId)
        {
            return await _context.Personels.FirstOrDefaultAsync(p => p.PersonelId == personelId);
        }

        public async Task<List<Personel>> GetAllAsync()
        {
            return await _context.Personels.ToListAsync();
        }

        public Personel GetPersonelByNameAndSurname(string firstName, string lastName)
        {
            return _context.Personels.FirstOrDefault(p => p.FirstName == firstName && p.LastName == lastName);
        }
        //alt üst birim sıralama
        //public async Task<Personel> GetByIdAsync(string personelId)
        //{
        //    return await _context.Personels
        //                         .Include(p => p.UnitPersonels)
        //                         .ThenInclude(up => up.Unit)
        //                         .FirstOrDefaultAsync(p => p.PersonelId == personelId);
        //}

        //public async Task<IEnumerable<Personel>> GetByFirstNameAsync(string firstName)
        //{
        //    return await _context.Personels
        //                         .Where(p => p.FirstName.Contains(firstName))
        //                         .Include(p => p.UnitPersonels)
        //                         .ThenInclude(up => up.Unit)
        //                         .ToListAsync();
        //}

        //public async Task<IEnumerable<Personel>> GetByLastNameAsync(string lastName)
        //{
        //    return await _context.Personels
        //                         .Where(p => p.LastName.Contains(lastName))
        //                         .Include(p => p.UnitPersonels)
        //                         .ThenInclude(up => up.Unit)
        //                         .ToListAsync();
        //}

        //public List<Personel> GetByUnitId(string unitId)
        //{
        //    return (from up in _context.UnitPersonels
        //            join p in _context.Personels on up.PersonelId equals p.PersonelId
        //            where up.UnitId == unitId
        //            select p).ToList();
        //}

    }
}
