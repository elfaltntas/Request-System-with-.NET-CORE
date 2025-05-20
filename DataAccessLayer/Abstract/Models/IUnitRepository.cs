using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.Models
{
    public interface IUnitRepository
    {
        // ID ile birimi getir
        //Task<Unit> GetByIdAsync(int id);
        Unit GetById(string unitId);
        IEnumerable<Unit> GetAll();
        Unit GetById(int id);
        void Add(Unit unit);
        void Update(Unit unit);
        void Delete(int id);
        public bool IsPersonelInUnit(string personelId, string unitId);
        
            List<Unit> GetAllParentUnits(string unitId);
        Task<Unit> GetByIdAsync(string unitId);
        Task<Unit> GetUnitByIdAsync(string departmentId);
        Task UpdateUnitAsync(Unit unit);
        IEnumerable<Unit> GetByName(string unitName);
        Task<string> FindIdByNameAsync(string unitName);

        IEnumerable<Unit> GetAllUnits();

        Task<Unit> AddUnitAsync(Unit unit);
        Task<Unit> GetUnitById(string unitId);
        Task<List<Unit>> GetAllUnitsAsync();
        Unit GetByNameA(string name);

        //Task<Unit> AddAsync(Unit unit);

        //Task SaveAsync();



        //Task<Unit> GetByIdAsync(int id);
        //Task<IEnumerable<Unit>> GetAllAsync();











        //// Değişiklikleri kaydet
        //void SaveChanges();
        //Unit GetUnitById(string unitId);
        //List<Unit> GetParentUnitId(string parentUnitId);
        ////List<Unit> GetParentUnits(string unitId);
        //string GetUnitIdByPersonelId(string personelId);
        //List<Unit> GetPersonelParentUnit(string unitId);
        //IEnumerable<Roles> GetRolesByUnitId(string unitId, int roleId);
        ////unit taşıma işlemi
        //Task<List<Unit>> GetChildrenByUnitIdAsync(string parentUnitId); // Alt birimleri getir
        ////unite ve personele göre listele
        //Task<IEnumerable<Unit>> GetAllAsync();
        //Task<Unit> GetByIdAsync(int unitId);     // ID ile birimi al
        //// Senkron GetByParentUnitId
        //IEnumerable<Unit> GetByParentUnitId(string parentUnitId);
        //// Senkron GetBySubUnitId
        //IEnumerable<Unit> GetBySubUnitId(string subUnitId);
    }
}
