using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.Models
{
    public interface IRequestProcessRepository
    {
        void Add(RequestProcess requestProcess);
        
        void CreateRequestProcess(RequestProcess requestProcess);
        void SaveChanges();
  
  
        List<RequestProcess> GetByRequestId(string requestId);
    
        RequestProcess GetLatestProcessByRequestId(string requestId);

        //void ReloadRequestProcess(string requestId); // Tanımlı olmalı
        //void Update(RequestProcess requestProcess);
        //void DetachEntity(RequestProcess requestProcess);
        //List<RequestProcess> GetByRequestId(string requestId);
        //Task<RequestProcess?> GetLatestProcessByRequestIdAsync(string requestId);
        //Task<IEnumerable<RequestProcess>> GetByRequestIdAsync(string requestId);
        //Task AddAsync(RequestProcess requestProcess);
        //Task SaveChangesAsync();

        //string GetEnumDescription(Enum value);

    }
}
