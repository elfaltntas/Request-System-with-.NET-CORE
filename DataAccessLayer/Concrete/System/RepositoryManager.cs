using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Abstract.System;
using DataAccessLayer.Concrete.Models;
using DataAccessLayer.Concrete.System; // Repository sınıflarınızın olduğu namespace
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.System
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly AppDbContext _context; // Veritabanı bağlamınız
        private readonly IRequestRepository _requestRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IPersonelRepository _personelRepository;
        private readonly IRequestProcessRepository _requestProcessRepository;
        private readonly IRoleRepository _rolesRepository;

        public RepositoryManager(AppDbContext context,
           IRequestRepository requestRepository,
            IUsersRepository usersRepository,
           IUnitRepository unitRepository,
           IPersonelRepository personelRepository,
           IRequestProcessRepository requestProcessRepository,
           IRoleRepository rolesRepository
            )
        {
            _context =context;
            _requestRepository = requestRepository;
            _usersRepository = usersRepository;
            _unitRepository = unitRepository;
            _personelRepository = personelRepository;
            _rolesRepository = rolesRepository;

        }

       
        public async Task SaveAsync()
                {
            await _context.SaveChangesAsync();
        }
    }
}
