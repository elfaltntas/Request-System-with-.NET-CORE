using Entities.Models;
using DataAccessLayer.Abstract.System;
using DataAccessLayer.Abstract.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DataAccessLayer.Concrete.System;
using System.Data;
//using DataAccessLayer.Migrations;

namespace DataAccessLayer.Concrete.Models
{
    public class UnitRepository :  IUnitRepository
    {
        private readonly AppDbContext _context;

        public UnitRepository(AppDbContext context) 
        {
            _context = context;
        }

       
        public Unit GetById(string unitId)
        {
            return _context.Units.FirstOrDefault(u => u.UnitId == unitId);
        }

        public List<Unit> GetAllParentUnits(string unitId)
        {
            var allParents = new List<Unit>();
            var currentUnit = _context.Units
                .Include(u => u.ParentUnit)
                .FirstOrDefault(u => u.UnitId == unitId);

            while (currentUnit?.ParentUnit != null)
            {
                // ParentUnit null olana kadar zinciri takip et
                var parent = _context.Units
                    .Include(u => u.Roles)
                    .FirstOrDefault(u => u.UnitId == currentUnit.ParentUnitId);

                if (parent != null)
                {
                    allParents.Add(parent);
                    currentUnit = parent;
                }
                else
                {
                    break;
                }
            }

            return allParents;
        }

        public IEnumerable<Unit> GetAll()
        {
            var units = _context.Units.ToList();
            if (units == null || !units.Any())
            {
                Console.WriteLine("No units found in the database.");
            }
            return units;
        }

        public Unit GetById(int id)
        {
            return _context.Units.Find(id);
        }

        public void Add(Unit unit)
        {
            _context.Units.Add(unit);
            _context.SaveChanges();
        }

        public void Update(Unit unit)
        {
            _context.Units.Update(unit);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var unit = _context.Units.Find(id);
            if (unit != null)
            {
                _context.Units.Remove(unit);
                _context.SaveChanges();
            }
        }
        //unit taşıyor
        public async Task<Unit> GetByIdAsync(string unitId)
        {
            return await _context.Units.FindAsync(unitId);
        }

        public async Task<Unit> GetUnitByIdAsync(string departmentId)
        {
            return await _context.Units.FindAsync(departmentId);
        }

        public async Task UpdateUnitAsync(Unit unit)
        {
            _context.Units.Update(unit);
            await _context.SaveChangesAsync();
        }
      

        public IEnumerable<Unit> GetByName(string name)
        {
            return _context.Units
                .Include(u => u.ParentUnit) // Üst birim ilişkisini dahil ediyoruz
                .Include(u => u.SubUnits)  // Alt birim ilişkisini dahil ediyoruz
       
                .ToList();
        }
        public async Task<string?> FindIdByNameAsync(string unitName)
        {
            var unit = await _context.Units.FirstOrDefaultAsync(u => u.Name == unitName);
            return unit?.UnitId;
        }
        public IEnumerable<Unit> GetAllUnits()
        {
            return _context.Units
                .Include(u => u.ParentUnit) // Üst birimi dahil etmek için
                .ToList();
        }

        public bool IsPersonelInUnit(string personelId, string unitId)
        {
            return _context.UnitPersonels.Any(up => up.PersonelId == personelId && up.UnitId == unitId);
        }

        public async Task<Unit> AddUnitAsync(Unit unit)
        {
            await _context.Set<Unit>().AddAsync(unit);
            await _context.SaveChangesAsync();
            return unit;
        }

        public async Task<Unit> GetUnitById(string unitId)
        {
            return await _context.Set<Unit>().FindAsync(unitId);
        }

        public async Task<List<Unit>> GetAllUnitsAsync()
        {
            return await _context.Set<Unit>().ToListAsync();
        }


        public Unit GetByNameA(string name)
        {
            return _context.Units.FirstOrDefault(u => u.Name == name);
        }

        //public async Task<Unit> AddAsync(Unit unit)
        //{
        //    await _context.Units.AddAsync(unit);
        //    return unit;
        //}

        //public async Task SaveAsync()
        //{
        //    await _context.SaveChangesAsync();
        //}
        //public async Task<Unit> GetByIdAsync(int id)
        //{
        //    return await _context.Units.FindAsync(id);
        //}

        //public async Task<IEnumerable<Unit>> GetAllAsync()
        //{
        //    return await _context.Units.ToListAsync();
        //}





        //public List<Unit> GetParentUnitId(string parentUnitId)
        //{
        //    return _context.Units.Where(u => u.ParentUnitId == parentUnitId).ToList();
        //}

        //public async Task<List<Unit>> GetChildrenByUnitIdAsync(string parentUnitId)
        //{
        //    return await _context.Units
        //        .Where(u => u.ParentUnitId == parentUnitId) // ParentUnitId'ye göre filtrele
        //        .ToListAsync();
        //}

        ////unitve personel e göre  listeleme
        //public async Task<IEnumerable<Unit>> GetAllAsync()
        //{
        //    return await _context.Units.ToListAsync();
        //}

        //public async Task<Unit> GetByIdAsync(int unitId)
        //{
        //    return await _context.Units.FindAsync(unitId);
        //}


        //// GetByParentUnitId metodunu senkron hale getirme
        //public IEnumerable<Unit> GetByParentUnitId(string parentUnitId)
        //{
        //    return _context.Units
        //        .Where(u => u.ParentUnitId == parentUnitId)
        //        .ToList();  // Asenkron olmadan direkt olarak ToList() kullanıyoruz
        //}

        //// GetBySubUnitId metodunu senkron hale getirme

        //public IEnumerable<Unit> GetBySubUnitId(string SubUnitId)
        //{
        //    // Alt birimleri almak için ParentUnitId'yi kullanıyoruz
        //    return _context.Units
        //    .Where(u => u.SubUnitId == SubUnitId)  // Parent birimi, alt birimlere ait olduğu için ParentUnitId'yi kontrol ediyoruz
        //        .ToList();
        //}

        //public string GetUnitIdByPersonelId(string personelId)
        //{
        //    var unit = _context.UnitPersonels
        //                       .Where(up => up.PersonelId == personelId)
        //                       .FirstOrDefault();
        //    return unit?.UnitId;
        //}

        //public List<Unit> GetPersonelParentUnit(string unitId)
        //{
        //    return _context.Units
        //                   .Where(u => u.ParentUnitId == unitId)
        //                   .ToList();
        //}
        //public Unit GetUnitById(string unitId)
        //{
        //    // UnitId'ye göre Unit nesnesini alıyoruz ve Unit döndürüyoruz.
        //    return _context.Units.FirstOrDefault(u => u.UnitId == unitId);
        //}

        //public IEnumerable<Roles> GetRolesByUnitId(string unitId, int roleId)
        //{
        //    // Birimi RoleId'ye göre alıyoruz
        //    var unit = _context.Units
        //        .Where(u => u.UnitId == unitId && u.RoleId == roleId)  // UnitId ve RoleId'yi kontrol ediyoruz
        //        .FirstOrDefault();

        //    if (unit == null)
        //    {
        //        return Enumerable.Empty<Roles>(); // RoleId == 1 olan birim bulunmazsa boş liste döndürüyoruz
        //    }

        //    // Unit'e karşılık gelen Rolleri alıyoruz
        //    return _context.Roles
        //        .Where(r => r.RoleId == roleId) // RoleId'yi kontrol ediyoruz
        //        .ToList();
        //}

        //public void SaveChanges()
        //{
        //    _context.SaveChanges();
        //}
    }
}
