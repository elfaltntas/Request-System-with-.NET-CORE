using DataAccessLayer.Abstract.Models;
using Entities.Dto;
using Entities.Models;
using Services.Abstract.ModelService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete.ModelManager
{
    public class PersonelUnitManager : IPersonelUnitService
    {
        private readonly IUnitPersonelRepository _unitPersonelRepository;
        private readonly IUnitRepository _unitRepository; // Birimleri almak için ekledik
        private readonly IPersonelRepository _personelRepository;

        public PersonelUnitManager(
            IUnitPersonelRepository unitPersonelRepository,
            IUnitRepository unitRepository,
            IPersonelRepository personelRepository)
        {
            _unitPersonelRepository = unitPersonelRepository;
            _unitRepository = unitRepository;
            _personelRepository = personelRepository;
        }

        public void AssignUnitToPersonel(PersonelUnitDto personelUnitDto)
        {
            // Personel kontrolü
            var personel = _personelRepository.GetById(personelUnitDto.PersonelId);
            if (personel == null)
            {
                throw new KeyNotFoundException("Personel bulunamadı.");
            }

            // Birim kontrolü
            var unit = _unitRepository.GetById(personelUnitDto.UnitId);
            if (unit == null)
            {
                throw new KeyNotFoundException("Birim bulunamadı.");
            }

            // Personel ve birim arasındaki ilişkiyi oluştur
            var unitPersonel = new UnitPersonel
            {
                PersonelId = personelUnitDto.PersonelId,
                UnitId = personelUnitDto.UnitId
                // Id burada belirtilmiyor çünkü veritabanı tarafından otomatik olarak atanacak
            };

            // UnitPersonel tablosuna kaydet
            _unitPersonelRepository.Add(unitPersonel);
            _unitPersonelRepository.SaveChanges();
        }


    }

}
