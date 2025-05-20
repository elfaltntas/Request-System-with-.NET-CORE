using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.Models

{
    public interface IRequestRepository
    {
        Request GetById(string RequestId);

        // Talep güncellemeyi sağlar
        void Update(Request request);
        void Add(Request request);
        void SaveChanges(Request request);
        List<RequestProcess> GetByRequestId(string requestId);
        IEnumerable<Request> Find(Func<Request, bool> predicate, Func<IQueryable<Request>, IQueryable<Request>> include = null);

        //List<Request> GetAll();
        //// Talep silmeyi sağlar
        //void Delete(Request request);
        // Request GetByIdRequest(string RequestId);
    }
}
