using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Concrete.Models;
using DataAccessLayer.Concrete.System;
using Entities.Dto;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Services.Abstract.ModelService;
using System;
using System.Threading.Tasks;

namespace Services.Concrete.ModelManager
{
    public class PersonelManager : IPersonelService
    {
        private readonly IPersonelRepository _personelRepository;
        private readonly IUnitPersonelRepository _unitPersonelRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly AppDbContext _context;

        public PersonelManager(IPersonelRepository personelRepository, 
            IUnitPersonelRepository unitPersonelRepository, 
            IUnitRepository unitRepository,
            IUsersRepository usersRepository,
            AppDbContext context)
        {
            _personelRepository = personelRepository;
            _unitPersonelRepository = unitPersonelRepository;
            _unitRepository = unitRepository;
            _usersRepository = usersRepository;
            _context = context;
        }

        public async Task AddPersonelAsync(Personel personel)
        {
            if (personel == null)
            {
                throw new ArgumentNullException(nameof(personel), "Personel verisi boş olamaz.");
            }
            personel.HireDate = DateTime.Now; // Mevcut tarih işe başlama tarihi olarak atanıyor
            await _personelRepository.AddAsync(personel); // Personel ekleme
            await _personelRepository.SaveChangesAsync(); // Değişiklikleri kaydet
        }

        public async Task SaveChangesAsync()
        {
            await _personelRepository.SaveChangesAsync();
        }


        //personel listeleme ancak isactive true ise sadece aktifler false ise aktif pasif hepsi sıralanıyor.
        public async Task<IEnumerable<PersonelWithUnitsDto>> GetAllPersonelsAsync(bool onlyActive = false)
        {
            // Veritabanından tüm personelleri alıyoruz
            var personels = _personelRepository.GetAll(onlyActive);

            // Personel ve birim nesnelerini DTO'ya dönüştürüyoruz
            var personelDtos = personels.Select(personel => new PersonelWithUnitsDto
            {
                PersonelId = personel.PersonelId,
                FirstName = personel.FirstName,
                LastName = personel.LastName,
                Units = personel.UnitPersonels
                    .Where(up => up.IsActive) // Sadece aktif birimleri al
                    .Select(unitPersonel => new UnitDto
                    {
                        Id = unitPersonel.UnitId,
                        UnitName = unitPersonel.Unit != null ? unitPersonel.Unit.Name : "Bilinmiyor"
                    }).ToList()
            }).ToList();

            return personelDtos;
        }


        // personel değiştirme
        public void AssignPersonelToUnit(string personelId, string newUnitId)
        {
            // Personel mevcut mu kontrol et
            var personel = _personelRepository.GetById(personelId);
            if (personel == null)
            {
                throw new KeyNotFoundException("Belirtilen personel bulunamadı.");
            }

            // Yeni birimi veritabanından al
            var newUnit = _unitRepository.GetById(newUnitId);
            if (newUnit == null)
            {
                throw new InvalidOperationException("Belirtilen birim bulunamadı.");
            }

            // Personelin mevcut birimlerini al
            var currentUnitPersonels = _unitPersonelRepository.GetByPersonelId(personelId);

            if (currentUnitPersonels == null || !currentUnitPersonels.Any())
            {
                // Eğer personelin birimi yoksa, yeni birim ataması yapılır
                var newUnitRecord = new UnitPersonel
                {
                    PersonelId = personelId,
                    UnitId = newUnitId,
                    IsActive = true, // Yeni birim aktif olacak
                    AssignedDate = DateTime.Now,
                    HireDate = DateTime.Now
                };

                _unitPersonelRepository.Add(newUnitRecord);  // Yeni birim kaydını ekle
                _unitPersonelRepository.SaveChanges();
                return;
            }

            // Mevcut birimleri kontrol et ve pasif yap
            var currentUnit = currentUnitPersonels.FirstOrDefault(up => up.IsActive);
            if (currentUnit != null)
            {
                currentUnit.IsActive = false;  // Mevcut birimi pasif yap
                currentUnit.DeactivationDate = DateTime.Now;
                _unitPersonelRepository.Update(currentUnit);  // Değişikliği kaydet
            }

            // Eğer yeni birim "Genel Müdürlük" (RoleId = 1) ise, mevcut müdür durumunu kontrol et
            if (newUnit.RoleId == "5371050b-1326-4e80-a906-a8cae91df572")
            {
                var currentManagerUnit = _unitPersonelRepository.GetByUnitId(newUnitId)
                    .FirstOrDefault(up => up.UnitId == newUnitId && up.IsActive);

                if (currentManagerUnit != null)
                {
                    currentManagerUnit.IsActive = false; // Mevcut müdürü pasif yap
                    currentManagerUnit.DeactivationDate = DateTime.Now;
                    _unitPersonelRepository.Update(currentManagerUnit); // Güncellemeyi kaydet
                }
            }

            // Yeni birimi aktif yap
            var newUnitPersonel = currentUnitPersonels
                .FirstOrDefault(up => up.UnitId == newUnitId && up.PersonelId == personelId);

            if (newUnitPersonel != null)
            {
                newUnitPersonel.IsActive = true;  // Yeni personeli aktif yap
                newUnitPersonel.AssignedDate = DateTime.Now; // Atama tarihini güncelle
                newUnitPersonel.ActiveDate = DateTime.Now;
                personel.ActiveDate = DateTime.Now;
                personel.IsActive = true;
                _unitPersonelRepository.Update(newUnitPersonel); // Güncellemeyi kaydet
            }
            else
            {
                // Yeni birim mevcut değilse, yeni birim ekle
                var newUnitRecord = new UnitPersonel
                {
                    PersonelId = personelId,
                    UnitId = newUnitId,
                    IsActive = true, // Yeni birim aktif olacak
                    AssignedDate = DateTime.Now,
                    HireDate= DateTime.Now,
                };

                _unitPersonelRepository.Add(newUnitRecord);  // Yeni birim kaydını ekle
            }

            // Değişiklikleri kaydet
            _unitPersonelRepository.SaveChanges();
        }



        //personel giriş çıkış bilgilerini gösteriyor.
        public async Task<PersonelStatusDto> GetPersonelStatusAsync(string personelId)
        {
            var personel = await _personelRepository.GetByIdAsync(personelId);
            if (personel == null) throw new KeyNotFoundException("Personel bulunamadı.");

            return new PersonelStatusDto
            {
                FullName = $"{personel.FirstName} {personel.LastName}",
                HireDate = personel.HireDate,
                TerminationDate = personel.TerminationDate,
                ActiveDate = personel.ActiveDate,
                DeactivationDate = personel.DeactivationDate,
                IsActive = personel.IsActive
            };
        }


        // tüm personellerin iş giirş çıkış bilgileri
        public async Task<List<PersonelStatusDto>> GetAllPersonelStatusesAsync()
        {
            var personels = await _personelRepository.GetAllAsync();

            return personels.Select(personel => new PersonelStatusDto
            {
                FullName = $"{personel.FirstName} {personel.LastName}",
                HireDate = personel.HireDate,
                TerminationDate = personel.TerminationDate,
                ActiveDate = personel.ActiveDate,
                DeactivationDate = personel.DeactivationDate,
                IsActive = personel.IsActive
            }).ToList();
        }
        public PersonelIdDto FindPersonelId(string firstName, string lastName)
        {
            var personel = _personelRepository.GetPersonelByNameAndSurname(firstName, lastName);

            if (personel == null)
            {
                return null;
            }

            return new PersonelIdDto
            {
               Id = personel.PersonelId
            };
        }


        public async Task<Personel> GetPersonelByIdAsync(string userId)
        {
            return await _context.Personels.FirstOrDefaultAsync(p => p.PersonelId == userId);
        }


        //personel İşten çıkarma
        //public async Task<string> TerminatePersonelAsync(string personelId)
        //{
        //    // Personeli veritabanında kontrol et
        //    var personel = _personelRepository.GetById(personelId);
        //    if (personel == null)
        //    {
        //        return "Personel bulunamadı.";
        //    }

        //    // Personelin aktifliğini pasif yap ve çıkış nedenini güncelle
        //    personel.IsActive = false;


        //    // Güncellemeleri kaydet
        //    _personelRepository.Update(personel);
        //    await _personelRepository.SaveChangesAsync();

        //    return "Personel başarıyla işten çıkarıldı.";
        //}

        public async Task<string> TerminatePersonelAsync(string personelId)
        {
            // Personeli kontrol et
            var personel = _personelRepository.GetById(personelId);
            if (personel == null)
            {
                return "Personel bulunamadı.";
            }

            // Personeli pasif yap
            personel.IsActive = false;
            personel.TerminationDate = DateTime.Now;
            personel.DeactivationDate = DateTime.Now;
            // Personelin birimlerdeki durumunu pasif yap
            var unitPersonels = _unitPersonelRepository.GetByPersonelId(personelId);
            foreach (var unitPersonel in unitPersonels)
            {
                unitPersonel.IsActive = false;
                unitPersonel.TerminationDate= DateTime.Now;
                unitPersonel.DeactivationDate = DateTime.Now;
                _unitPersonelRepository.Update(unitPersonel);
            }

            //Kullanıcıyı kontrol et ve personel durumunu false yap
           var user = _usersRepository.GetByUsername(personel.Username);
            if (user != null)
            {
                user.IsPersonel = false;
                _usersRepository.Update(user);
            }

            // Tüm değişiklikleri kaydet
            _personelRepository.Update(personel);
            await _unitPersonelRepository.SaveChangesAsync();
            await _personelRepository.SaveChangesAsync();
            await _usersRepository.SaveChangesAsync();

            return "Personel başarıyla işten çıkarıldı.";
        }





        //public void AssignPersonelToUnit(string personelId, string newUnitId)
        //{
        //    // Personel mevcut mu kontrol et
        //    var personel = _personelRepository.GetById(personelId);
        //    if (personel == null)
        //    {
        //        throw new KeyNotFoundException("Belirtilen personel bulunamadı.");
        //    }

        //    // Personelin mevcut birimlerini al
        //    var currentUnitPersonels = _unitPersonelRepository.GetByPersonelId(personelId);

        //    // Eğer mevcut birimler varsa
        //    if (currentUnitPersonels != null && currentUnitPersonels.Any())
        //    {
        //        // Şu anda aktif olan birimi kontrol et ve pasif yap
        //        var currentActiveUnit = currentUnitPersonels.FirstOrDefault(up => up.IsActive);

        //        if (currentActiveUnit != null)
        //        {
        //            // Eğer aktif birim varsa, onu pasif yap
        //            currentActiveUnit.IsActive = false; // Aktif birimi pasif yap
        //            _unitPersonelRepository.Update(currentActiveUnit); // Değişikliği kaydet
        //            _unitPersonelRepository.SaveChanges();
        //        }

        //        // Yeni birim mevcutsa, onu aktif yap
        //        var newUnitPersonel = currentUnitPersonels.FirstOrDefault(up => up.UnitId == newUnitId);

        //        if (newUnitPersonel != null)
        //        {
        //            // Eğer yeni birim mevcutsa, aktif yap
        //            newUnitPersonel.IsActive = true;  // Yeni birimi aktif yap
        //            newUnitPersonel.AssignedDate = DateTime.Now; // Atama tarihini güncelle
        //            _unitPersonelRepository.Update(newUnitPersonel); // Değişikliği kaydet
        //        }
        //        else
        //        {
        //            // Eğer yeni birim mevcut değilse, yeni birim ekleyelim
        //            var newUnit = new UnitPersonel
        //            {
        //                PersonelId = personelId,
        //                UnitId = newUnitId,
        //                IsActive = true, // Yeni birim aktif olacak
        //                AssignedDate = DateTime.Now
        //            };

        //            _unitPersonelRepository.Add(newUnit); // Yeni birim kaydını ekle
        //        }


        //        // Değişiklikleri kaydet
        //        _unitPersonelRepository.SaveChanges();
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException("Personelin birimi bulunamadı.");
        //    }
        //}



        //public Task<Personel> GetPersonelByNameAsync(string firstName, string lastName)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<Personel> GetPersonelByNameAsync(string firstName, string lastName)
        //{
        //    //var personel = await _personelRepository.Query()
        //    //    .Include(p => p.UnitPersonels) // Personelin birimleri
        //    //        .ThenInclude(up => up.Unit) // Ara tablo üzerinden birim bilgisi
        //    //    .FirstOrDefaultAsync(p => p.FirstName == firstName && p.LastName == lastName);

        //    //if (personel == null)
        //    //    throw new KeyNotFoundException($"Personel bulunamadı: {firstName} {lastName}");

        //    //return personel;
        //}

        //personele göre unitle sıralıyor

    }
}
