using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Concrete.System;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.Models
{
    public class UnitPersonelRepository : IUnitPersonelRepository
    {
        private readonly AppDbContext _context;

        public UnitPersonelRepository(AppDbContext context)
        {
            _context = context;
        }

        public string GetUnitIdByPersonelId(string personelId)
        {
            var unitPersonel = _context.UnitPersonels
                .FirstOrDefault(up => up.PersonelId == personelId);

            return unitPersonel?.UnitId; // null kontrolü ile UnitId döndürülür
        }

        public void SaveChanges()
        {
            _context.SaveChanges();  // Senkron şekilde veritabanı değişikliklerini kaydeder.
        }




        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Add(UnitPersonel unitPersonel)
        {
            _context.UnitPersonels.Add(unitPersonel); // UnitPersonel nesnesini veritabanına ekler.
        }
       
        public IEnumerable<Personel> GetPersonelsByUnitId(string unitId)
        {
            return _context.UnitPersonels
                .Where(up => up.UnitId == unitId)
                .Select(up => up.Personel)
                .ToList();
        }

        public IEnumerable<UnitPersonel> GetByPersonelId(string personelId)
        {
            return _context.UnitPersonels
                .Include(up => up.Personel)
                .Include(up => up.Unit)
                .Where(up => up.PersonelId == personelId)
                .ToList(); // Elde edilen listeyi döndürür
        }
      
        // UnitPersonel'i güncelle
        public void Update(UnitPersonel unitPersonel)
        {
            var entry = _context.Entry(unitPersonel);
            entry.State = EntityState.Modified; // UnitPersonel nesnesini güncelle olarak işaretle
            _context.SaveChanges(); // Veritabanında kaydet
        }

        public IEnumerable<UnitPersonel> GetByUnitId(string unitId)
        {
            return _context.UnitPersonels.Where(up => up.UnitId == unitId).ToList();
        }

        public void Delete(UnitPersonel unitPersonel)
        {
            _context.UnitPersonels.Remove(unitPersonel);
        }
        public IEnumerable<Personel> GetPersonelById(string personelId)
        {
            return _context.Personels.Where(up => up.PersonelId == personelId).ToList();
        }

        public void DeletePersonel(Personel personel)
        {
            _context.Personels.Remove(personel);
        }




        //public string GetUnitManagerId(string unitId)
        //{
        //    var unit = _context.Units.FirstOrDefault(u => u.UnitId == unitId);
        //    return unit.UnitId; // Birimin yöneticisinin ID'si

        //}

        //public List<UnitPersonel> GetByUnitId(string unitId)
        //{
        //    // UnitId'ye bağlı personelleri alıyoruz
        //    var unitPersonels = _context.UnitPersonels
        //                                 .Where(up => up.UnitId == unitId)
        //                                 .ToList();
        //    return unitPersonels;
        //}
    }

}
