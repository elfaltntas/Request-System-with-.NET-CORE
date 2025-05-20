using DataAccessLayer.Abstract.Models;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.System
{
    public interface IRepositoryManager
    {

        //IRequestRepository RequestRepository { get; }
        //IUnitRepository UnitRepository { get; }
        //IUsersRepository UsersRepository { get; }
        //IPersonelRepository PersonelRepository { get; }
        //IRequestProcessRepository RequestProcessRepository { get; }
        //IRolesRepository RolesRepository { get; }
        // Değişiklikleri kaydet
        Task SaveAsync();

       
    }
}
