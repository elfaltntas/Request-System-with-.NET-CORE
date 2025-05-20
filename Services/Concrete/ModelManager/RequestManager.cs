using Azure.Core;
using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Abstract.System;
using DataAccessLayer.Concrete.Models;
using DataAccessLayer.Concrete.System;
using Entities.Dto;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.Abstract.ModelService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Concrete.ModelManager
{
    public class RequestManager : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IPersonelRepository _personelRepository;
        private readonly IPersonelRoleRepository _personelRoleRepository;
        private readonly AppDbContext _context;
        private readonly IRequestProcessRepository _requestProcessRepository;
        private readonly IUnitPersonelRepository _unitPersonelRepository;
        private readonly IEnumHelper _enumHelper;
        private readonly IUserService _userService;
        //private readonly ICacheService _cache;

        public RequestManager(
            IRequestRepository requestRepository,
            IUnitRepository unitRepository,
            IRequestProcessRepository requestProcessRepository,
            IPersonelRepository personelRepository,
            AppDbContext context,
             IUnitPersonelRepository unitPersonelRepository, 
             //ICacheService cache, 
             IEnumHelper enumHelper, IUserService userService, IPersonelRoleRepository personelRoleRepository
             )

        {
            _requestRepository = requestRepository;
            _unitRepository = unitRepository;
            _personelRepository = personelRepository;
            _context = context;
            _requestProcessRepository = requestProcessRepository;
            _unitPersonelRepository = unitPersonelRepository;
            _enumHelper = enumHelper;
            _userService = userService;
            _personelRoleRepository = personelRoleRepository;
            //_cache = cache;
           

        }

        

        //personel id ye göre request getiriyor
        public IEnumerable<RequestUserDto> GetRequestsByUser(string personelId)
        {
            // Veritabanından personel ID'ye göre talepleri sorgula
            var requests = _requestRepository.Find(r => r.PersonelId == personelId, include: query => query.Include(r => r.Personel));

            // Talepleri DTO'ya dönüştür
            return requests.Select(r => new RequestUserDto
            {
                RequestId = r.RequestId,
                Title = r.Title,
                Description = r.Description,
                FirstName = r.Personel.FirstName,
                LastName = r.Personel.LastName,
                Status = r.RequestStatus.ToString(),
                CreatedDate = r.CreatedDate,
                ModifiedDate = r.ModifiedDate,
            });
        }



        //talep durumunu gösteriyor
        public string GetRequestProcessStatusDescription(string requestId)
        {
            // Orijinal implementasyon buraya gelecek
            var latestRequestProcess = _requestProcessRepository.GetLatestProcessByRequestId(requestId);

            if (latestRequestProcess == null)
            {
                throw new KeyNotFoundException("Talep için işlem kaydı bulunamadı.");
            }

            if (latestRequestProcess.Status == RequestStatus.Cancelled)
            {
                return $"Talep iptal edildi. Sebep: {latestRequestProcess.Notes}";
            }

            return GetEnumDescription(latestRequestProcess.Status);
        }


        // Enum değerini açıklamaya çeviren yardımcı metot
        private string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (System.ComponentModel.DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }


        //  burada tüm üst birimleri tarayıp rol id si 1 olana kadar gidiyor
        public Unit FindManagerialUnitInHierarchy(string unitId)
        {
            // Tüm üst birimleri al
            var allParents = _unitRepository.GetAllParentUnits(unitId);

            // Mevcut birimi kontrol et
            var currentUnit = _unitRepository.GetById(unitId);
            if (currentUnit != null && currentUnit.RoleId == "2c5e4f53-b77c-4cff-9fb5-dbf45802fb4f") // Müdürlük rolü kontrolü
            {
                return currentUnit; // Eğer mevcut birim müdürlükse döndür
            }

            // Eğer üst birimler varsa, ilk müdürlük birimini bul ve döndür
            if (allParents != null && allParents.Any())
            {
                foreach (var parent in allParents)
                {
                    if (parent.RoleId == "2c5e4f53-b77c-4cff-9fb5-dbf45802fb4f") // Müdürlük rolü kontrolü
                    {
                        return parent; // İlk müdürlük birimini döndür
                    }
                }
            }

            // Üst birimlerde müdürlük bulunamazsa, en üst düzey birime kadar recursive kontrol
            var topLevelUnit = allParents?.FirstOrDefault();
            if (topLevelUnit != null && !string.IsNullOrEmpty(topLevelUnit.ParentUnitId))
            {
                return FindManagerialUnitInHierarchy(topLevelUnit.ParentUnitId);
            }

            // Eğer hiçbir müdürlük birimi bulunamazsa null döner
            return null;
        }


        // personele talep oluşturuyor
        public void CreateRequest(RequestPersonelDto requestPersonelDto)
        {
            try
            {

                // Personeli al
                var personel = _personelRepository.GetByFullName(requestPersonelDto.FirstName, requestPersonelDto.LastName);
                if (personel == null)
                {
                    throw new KeyNotFoundException("Belirtilen isimde bir personel bulunamadı.");
                }

                // Personelin birimini al
                var unitId = _unitPersonelRepository.GetUnitIdByPersonelId(personel.PersonelId);
                if (string.IsNullOrEmpty(unitId))
                {
                    throw new KeyNotFoundException("Personelin birimi bulunamadı.");
                }

                // Talep oluşturma
                var request = new Entities.Models.Request
                {
                    RequestId = Guid.NewGuid().ToString(), // Benzersiz RequestId oluşturuluyor
                    Title = requestPersonelDto.Title,
                    Description = requestPersonelDto.Description,
                    PersonelId = personel.PersonelId,
                };

                // Talep kaydını veritabanına ekle
                _requestRepository.Add(request);
                _requestRepository.SaveChanges(request); // SaveChanges çağrısı yapıyoruz

                // "Müdürlük" rolüne sahip bir üst birimi buluyoruz
                var managerialUnit = FindManagerialUnitInHierarchy(unitId);

                // 5. Müdürlük birimindeki personelleri al
                var personnelsInUnit = _unitPersonelRepository.GetPersonelsByUnitId(managerialUnit.UnitId);

                // 6. Müdür olan personeli bul (PersonelRoles tablosuna göre)
                var manager = personnelsInUnit.FirstOrDefault(p =>
                    _personelRoleRepository.HasRole(p.PersonelId, "2c5e4f53-b77c-4cff-9fb5-dbf45802fb4f") // "1" => Müdür rolü ID'si
                );

                if (manager == null)
                    throw new Exception("Müdürlük biriminde müdür bulunamadı.");



                // Talebi bu birime gönder
                var requestProcessForManager = new RequestProcess
                {
                    RequestId = request.RequestId, // RequestId'yi burada alıyoruz
                    Status = RequestStatus.Pending, // Durum 'Pending' olarak ayarlandı
                    UnitId = managerialUnit.UnitId, // "Müdürlük" rolüne sahip birimin ID'si
                    PersonelId = manager.PersonelId,// ✅ Müdürün ID’si buraya geldi
                    CreatedDate = DateTime.Now // <== BURAYA EKLE
                };

                // RequestProcess nesnesini veritabanına ekle
                _requestProcessRepository.CreateRequestProcess(requestProcessForManager);

                // RequestProcess'i kaydet
                _requestProcessRepository.SaveChanges();

                // Eğer burada hata meydana gelirse, exception fırlatılacaktır
                Console.WriteLine($"RequestProcess başarıyla oluşturuldu. Talep ID: {request.RequestId}, Process ID: {requestProcessForManager.RequestId}");
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                throw; // İstediğiniz başka bir işlem yapılabilir
            }
        }


        //talep yönlendirme kısmı
        public void RedirectRequest(RedirectRequestDto redirectRequestDto)
        {
            // Talep veritabanından alınır
            var request = _requestRepository.GetById(redirectRequestDto.RequestId);
            if (request == null)
            {
                throw new KeyNotFoundException("Belirtilen talep bulunamadı.");
            }

            // Önceki süreç kaydı alınabilir (ama güncellenmeyecek!)
            var requestProcesses = _requestRepository.GetByRequestId(redirectRequestDto.RequestId);
            var previousProcess = requestProcesses.OrderByDescending(p => p.CreatedDate).FirstOrDefault();

            if (previousProcess == null)
            {
                throw new KeyNotFoundException("Bu talep için işlem kaydı bulunamadı.");
            }

            // Yeni birim ve personel bilgileri
            var unitId = redirectRequestDto.NewUnitId ?? previousProcess.UnitId;
            var personelId = redirectRequestDto.TargetPersonelId ?? previousProcess.PersonelId;

            // Personelin birime ait olup olmadığını kontrol et
            var isPersonelInUnit = _unitRepository.IsPersonelInUnit(personelId, unitId);
            if (!isPersonelInUnit)
            {
                throw new InvalidOperationException("Seçilen personel belirtilen birime ait değildir.");
            }

            // Yeni bir RequestProcess kaydı oluşturuluyor
            var newRequestProcess = new RequestProcess
            {
                RequestId = redirectRequestDto.RequestId,
                UnitId = unitId,
                PersonelId = personelId,
                Status = RequestStatus.InProgress,
                Notes = redirectRequestDto.Notes,
                CreatedDate = DateTime.Now
            };

            // Yeni süreci ekle
            _requestProcessRepository.Add(newRequestProcess);

            // Request tablosunu güncelle
            request.RequestStatus = newRequestProcess.Status;
            request.PersonelId = personelId; // PersonelId'yi güncelle
            _requestRepository.Update(request);

            // Değişiklikleri kaydet
            _requestRepository.SaveChanges(request); // Request değişikliklerini kaydet
            _requestProcessRepository.SaveChanges(); // RequestProcess değişikliklerini kaydet

            // Context'teki değişiklikleri temizle
            _context.ChangeTracker.Clear();

            // Doğrulama işlemi
            var dogrulama = _requestProcessRepository.GetLatestProcessByRequestId(redirectRequestDto.RequestId);
            Console.WriteLine($"Doğrulama: En son durum = {dogrulama.Status}, Not = {dogrulama.Notes}, Tarih = {dogrulama.CreatedDate}");

            // Eğer gerekiyorsa loglama
            Console.WriteLine($"Talep yönlendirildi. Yeni süreç oluşturuldu. Talep ID: {newRequestProcess.RequestId}, Birim: {newRequestProcess.UnitId}, Personel: {newRequestProcess.PersonelId}");
        }




        //personelin talebi onaylayıp onaylamadığını kontrol ediyor
        public void UpdateRequestStatus(UpdateRequestStatusDto updateRequestStatusDto)
        {
            //_cache.Remove($"RequestProcess_{updateRequestStatusDto.RequestId}");
            // Talep kaydını al
            var request = _requestRepository.GetById(updateRequestStatusDto.RequestId);
            if (request == null)
            {
                throw new KeyNotFoundException("Belirtilen talep bulunamadı.");
            }

            // Önceki süreç kaydını al (isteğe bağlı olarak kullanılabilir)
            var lastRequestProcess = _requestProcessRepository.GetByRequestId(updateRequestStatusDto.RequestId).OrderByDescending(rp => rp.CreatedDate).FirstOrDefault();

            // Talep durumu güncelleniyor
            request.RequestStatus = updateRequestStatusDto.NewStatus;
            _requestRepository.Update(request);
            _requestRepository.SaveChanges(request);

            // Yeni requestProcess oluşturuluyor
            var newRequestProcess = new RequestProcess
            {
                RequestId = request.RequestId,
                UnitId = lastRequestProcess?.UnitId, // Önceki birim varsa devam etsin
                PersonelId = lastRequestProcess?.PersonelId, // Aynı personel devam etsin
                Status = updateRequestStatusDto.NewStatus,
                Notes = updateRequestStatusDto.Notes,
                CreatedDate = DateTime.Now
            };

            _requestProcessRepository.Add(newRequestProcess);
          
            _requestProcessRepository.SaveChanges();

            _context.ChangeTracker.Clear();//cache temizliyor
            var dogrulama = _requestProcessRepository.GetLatestProcessByRequestId(updateRequestStatusDto.RequestId);
          


            // Ek bilgi mesajı
            if (updateRequestStatusDto.NewStatus == RequestStatus.Cancelled)
            {
                Console.WriteLine($"Talep iptal edildi. Gerekçe: {updateRequestStatusDto.Notes}");
            }
        }

      
    }
}
