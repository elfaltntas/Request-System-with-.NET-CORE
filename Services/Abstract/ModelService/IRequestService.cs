using Entities.Dto;
using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstract.ModelService
{
    public interface IRequestService
    {

        IEnumerable<RequestUserDto> GetRequestsByUser(string personelId); // Kullanıcının taleplerini listele

        string GetRequestProcessStatusDescription(string requestId);


        void CreateRequest(RequestPersonelDto requestPersonelDto); // Yeni talep oluştur

        void RedirectRequest(RedirectRequestDto redirectRequestDto);
        void UpdateRequestStatus(UpdateRequestStatusDto updateRequestStatusDto);
        //Task<string> GetRequestProcessStatusDescriptionAsync(string requestId);

    }
}
