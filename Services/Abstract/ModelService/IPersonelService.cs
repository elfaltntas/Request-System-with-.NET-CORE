using Entities.Dto;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract.ModelService
{
    public interface IPersonelService
    {
        //    Task RegisterPersonelAsync(PersonelForRegistrationDto personelForRegistrationDto);
        //    Task<bool> ValidatePersonelAsync(PersonelForAuthenticationDto personelForAuthDto);
        //    Task<Personel> GetPersonelByEmailAsync(string email);
        Task AddPersonelAsync(Personel personel);
        Task SaveChangesAsync();
        //Task<Personel> GetPersonelByNameAsync(string firstName, string lastName);
        Task<IEnumerable<PersonelWithUnitsDto>> GetAllPersonelsAsync(bool onlyActive = false);
        void AssignPersonelToUnit(string personelId, string newUnitId);
        Task<string> TerminatePersonelAsync(string personelId);

        Task<PersonelStatusDto> GetPersonelStatusAsync(string personelId);
        Task<List<PersonelStatusDto>> GetAllPersonelStatusesAsync();
        Task<Personel> GetPersonelByIdAsync(string userId);
        PersonelIdDto FindPersonelId(string firstName, string lastName);
    }
}
