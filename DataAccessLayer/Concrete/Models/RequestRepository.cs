using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Abstract.System;
using DataAccessLayer.Concrete.System;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.Models
{
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Request> _dbSet;

        public RequestRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<Request>();
        }

        public Request GetById(string Requestid)
        {
            return _dbSet.Include(r => r.Personel)
                         .ThenInclude(p => p.UnitPersonels)
                         .FirstOrDefault(r => r.RequestId == Requestid);

        }
 
        public List<RequestProcess> GetByRequestId(string requestId)
        {
            return _context.RequestProcesses.Where(rp => rp.RequestId == requestId).ToList();
        }
        public void Add(Request request)
        {
            _context.Requests.Add(request);
        }

        public void SaveChanges(Request request)
        {
            _context.SaveChanges();
        }

        public IEnumerable<Request> Find(Func<Request, bool> predicate, Func<IQueryable<Request>, IQueryable<Request>> include = null)
        {
            IQueryable<Request> query = _context.Set<Request>();

            // Eğer 'include' parametresi sağlanmışsa, ilişkili verileri dahil et
            if (include != null)
            {
                query = include(query); // Include işlemleri eklenir
            }

            // Predicate ile filtreleme yapılır ve sonuç döndürülür
            return query.Where(predicate).ToList();
        }

        public void Update(Request request)
        {
            _context.Update(request);
        }







        //public Request GetByIdRequest(string requestId)
        //{
        //    return _context.Requests.FirstOrDefault(r => r.RequestId == requestId);
        //}

        //public void Delete(Request request)
        //{
        //    _context.SaveChanges();
        //}

        //public List<Request> GetAll()
        //{
        //    return _context.Requests.ToList();
        //}
    }
}
