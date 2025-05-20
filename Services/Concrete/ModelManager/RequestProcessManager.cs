using DataAccessLayer.Abstract.Models;
using Entities.Models;
using Services.Abstract.ModelService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete.ModelManager
{
    public class RequestProcessManager : IRequestProcess
    {
        private readonly IRequestProcessRepository _requestProcessRepository;

        public RequestProcessManager(IRequestProcessRepository requestProcessRepository)
        {
            _requestProcessRepository = requestProcessRepository;
        }

        public void CreateRequestProcess(RequestProcess requestProcess)
        {
            _requestProcessRepository.Add(requestProcess);  // Add metodunu senkron olarak çağır
            _requestProcessRepository.SaveChanges();        // SaveChanges metodunu senkron olarak çağır
        }

    }

}
