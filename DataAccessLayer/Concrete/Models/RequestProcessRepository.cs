using Azure.Core;
using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Concrete.System;
using Entities.EntityType.Abstract;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.Models
{
    public class RequestProcessRepository : IRequestProcessRepository
    {
        private readonly AppDbContext _context;

        public RequestProcessRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(RequestProcess requestProcess)
        {
            requestProcess.CreatedDate = DateTime.Now;
            requestProcess.ModifiedDate = DateTime.Now;

            _context.RequestProcesses.Add(requestProcess);
        }
        
        public void CreateRequestProcess(RequestProcess requestProcess)
        {
            if (requestProcess == null)
            {
                throw new ArgumentNullException(nameof(requestProcess), "RequestProcess nesnesi boş olamaz.");
            }

            try
            {
                // RequestProcess nesnesini veritabanına ekle
                // RequestProcess nesnesini veritabanına ekle
                requestProcess.CreatedDate = DateTime.Now; // İlk oluşturma zamanı
                requestProcess.ModifiedDate = DateTime.Now; // Son güncelleme zamanı
                requestProcess.IsActive = true;            // Varsayılan aktif durum
                requestProcess.IsDeleted = false;          // Varsayılan silinme durumu

                _context.RequestProcesses.Add(requestProcess);

                // Değişiklikleri kaydet
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("RequestProcess oluşturulurken bir hata oluştu.", ex);
            }
        }

        public List<RequestProcess> GetByRequestId(string requestId)
        {
            return _context.RequestProcesses
        .AsNoTracking() // Güncel veriyi almak için izleme yapmıyoruz
        .Where(rp => rp.RequestId == requestId)
        .ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        


        public RequestProcess GetLatestProcessByRequestId(string requestId)
        {
            return _context.RequestProcesses
                .AsNoTracking()
                .Where(rp => rp.RequestId == requestId)
                .OrderByDescending(rp => rp.CreatedDate)
                .FirstOrDefault();
        }
    }
}

//public void ReloadRequestProcess(string requestId)
//{
//    var requestProcess = _context.RequestProcesses.FirstOrDefault(rp => rp.RequestId == requestId);
//    if (requestProcess != null)
//    {
//        _context.Entry(requestProcess).Reload();
//    }
//}


//public void Update(RequestProcess requestProcess)
//{
//    _context.RequestProcesses.Update(requestProcess);
//    _context.SaveChanges(); // Değişiklikleri kaydet
//}

//   public async Task<RequestProcess?> GetLatestProcessByRequestIdAsync(string requestId)
//{
//    return await _context.RequestProcesses
//        .AsNoTracking()
//        .Where(rp => rp.RequestId == requestId)
//        .OrderByDescending(rp => rp.CreatedDate)
//        .FirstOrDefaultAsync();
//}

//public IEnumerable<RequestProcess> GetByRequestId(string requestId)
//{
//    return _context.RequestProcesses
//        .Where(rp => rp.RequestId == requestId)
//        .OrderByDescending(rp => rp.CreatedDate)
//        .ToList(); // ToListAsync() yerine ToList()
//}


//public void DetachEntity(RequestProcess requestProcess)
//{
//    _context.Entry(requestProcess).State = EntityState.Detached;
//}




