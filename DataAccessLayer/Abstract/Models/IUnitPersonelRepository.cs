using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.Models
{
    public interface IUnitPersonelRepository
    {
        string GetUnitIdByPersonelId(string personelId);
        //string GetUnitManagerId(string unitId); // Birim yöneticisini al
        //                                        //Task<UnitPersonel> GetByPersonelIdAsync(string personelId);
        void Add(UnitPersonel unitPersonel);
        //List<UnitPersonel> GetByUnitId(string unitId);
        // Değişiklikleri kaydetmek için senkron metod
        void SaveChanges();
        Task SaveChangesAsync();
        void Delete(UnitPersonel unitPersonel); 
        IEnumerable<Personel> GetPersonelsByUnitId(string unitId);
        IEnumerable<UnitPersonel> GetByPersonelId(string personelId);
        void Update(UnitPersonel unitPersonel);
        IEnumerable<UnitPersonel> GetByUnitId(string unitId);
        IEnumerable<Personel> GetPersonelById(string personelId);
        void DeletePersonel(Personel personel);


    }
}
