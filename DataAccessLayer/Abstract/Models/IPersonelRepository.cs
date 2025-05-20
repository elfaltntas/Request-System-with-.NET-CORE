using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.Models
{
    public interface IPersonelRepository
    {
        // Personel ID ile personeli getir
        Personel GetById(string personelId);

        // Yeni personel ekle
        Task AddAsync(Personel personel);

        Personel GetByFullName(string firstName, string lastName);

        // Veritabanı değişikliklerini kaydet
        Task SaveChangesAsync();
        IEnumerable<Personel> GetAll(bool onlyActive = false);
        void Update(Personel personel);

        Task<Personel> GetByIdAsync(string id);
        Task<List<Personel>> GetAllAsync();
        Personel GetPersonelByNameAndSurname(string firstName, string lastName);


        //List<Personel> GetByUnitId(string unitId);
        //Task<Personel> GetByIdAsync(string personelId);   // Personel ID ile personel al
        //Task<IEnumerable<Personel>> GetByFirstNameAsync(string firstName);  // İlk isme göre personel al
        //Task<IEnumerable<Personel>> GetByLastNameAsync(string lastName);    // Soy isme göre personel al

        // Personel bilgilerini güncelle
        //Task UpdateAsync(Personel personel);

        // Personel sil
        //Task DeleteAsync(Personel personel);

        //IQueryable<Personel> Query(); // Personeller üzerinde sorgulama
        // Tüm personelleri getir
        //Task<IEnumerable<Personel>> GetAllAsync();

        // Şarta göre personelleri getir
        //Task<IEnumerable<Personel>> FindAsync(Expression<Func<Personel, bool>> predicate);

    }
}
