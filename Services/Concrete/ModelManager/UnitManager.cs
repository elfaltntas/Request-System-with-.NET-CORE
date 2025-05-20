using AutoMapper;
using DataAccessLayer.Abstract.Models;
using DataAccessLayer.Concrete.Models;
using DataAccessLayer.Concrete.System;
using Entities.Dto;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Abstract.ModelService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete.ModelManager
{
    public class UnitManager : IUnitService
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly ILogger<Unit> _logger;
        private readonly IPersonelRepository _personelRepository;
        public UnitManager(IUnitRepository unitRepository, IMapper mapper, AppDbContext context, ILogger<Unit> logger, IPersonelRepository personelRepository)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
            _context = context;
            _logger = logger;
            _personelRepository = personelRepository;
        }

        public async Task<bool> ChangeUnitParentAsync(MoveUnitWithChildrenDto dto)
        {
            try
            {
                var unit = await _unitRepository.GetByIdAsync(dto.UnitId);
                var newParent = await _unitRepository.GetUnitByIdAsync(dto.NewParentDepartmentId);

                if (unit == null || newParent == null)
                {
                    _logger.LogWarning("Invalid Unit ID: {UnitId} or Department ID: {DepartmentId}", dto.UnitId, dto.NewParentDepartmentId);
                    return false;
                }

                unit.ParentUnitId = dto.NewParentDepartmentId;
                await _unitRepository.UpdateUnitAsync(unit);

                _logger.LogInformation("Unit {UnitId} moved to Department {DepartmentId}", dto.UnitId, dto.NewParentDepartmentId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing unit parent");
                throw; // Global exception handler'a bırak
            }
        }

  
        //personel ismine göre alt üst birim getirme
        public IEnumerable<UnitWithParentChildDto> GetUnitsByPersonel(string firstName, string lastName)
        {
            var personel = _personelRepository.GetAll()
                .FirstOrDefault(p => p.FirstName == firstName && p.LastName == lastName);

            if (personel == null)
            {
                throw new KeyNotFoundException("Personel bulunamadı.");
            }

            // Get the first unit associated with the person
            var firstUnit = personel.UnitPersonels.Select(up => up.Unit).FirstOrDefault();

            if (firstUnit == null)
            {
                throw new KeyNotFoundException("Personelin atanmış bir birimi yok.");
            }

            // Traverse the hierarchy
            var parentUnits = GetAllParentUnitsRecursive(firstUnit).ToList();
            var subUnits = GetAllSubUnitsRecursive(firstUnit).ToList();

            // Create the DTO
            return new List<UnitWithParentChildDto>
    {
                new UnitWithParentChildDto
                {
                    UnitName = firstUnit.Name,
                    ParentUnits = parentUnits,
                    SubUnits = subUnits
                }
    };
        }

        //unit ismine göre alt üst birim getiriyor
        public IEnumerable<UnitWithParentChildDto> GetUnitsByUnitName(string unitName)
        {
            // Verilen isimle eşleşen birimi alıyoruz
            var unit = _unitRepository.GetByName(unitName).FirstOrDefault(p => p.Name == unitName);

            if (unit == null)
            {
                // Eğer birim bulunamazsa, hata fırlatıyoruz
                throw new KeyNotFoundException("Birim bulunamadı.");
            }

            // Alt birimleri almak için recursive metodu kullanıyoruz
            var subUnits = GetAllSubUnitsRecursive(unit).ToList();

            // Üst birimleri almak için recursive metodu kullanıyoruz
            var parentUnits = GetAllParentUnitsRecursive(unit).ToList();

            // DTO oluşturuyoruz
            return new List<UnitWithParentChildDto>
    {
        new UnitWithParentChildDto
        {
            UnitName = unit.Name, // Ana birimin adı
            ParentUnits = parentUnits, // Üst birimleri alıyoruz
            SubUnits = subUnits // Alt birimleri alıyoruz
        }
    };
        }


        // yardımcı metodlar 
        private IEnumerable<string> GetAllParentUnitsRecursive(Unit unit)
        {
            var parents = new List<string>();
            var currentUnit = unit;

            while (currentUnit.ParentUnit != null)
            {
                parents.Add(currentUnit.ParentUnit.Name);
                currentUnit = currentUnit.ParentUnit;
            }

            return parents;
        }

        
        private IEnumerable<string> GetAllSubUnitsRecursive(Unit unit)
        {
            var subUnits = new List<string>();

            if (unit.SubUnits == null || !unit.SubUnits.Any())
            {
                return subUnits;
            }

            foreach (var subUnit in unit.SubUnits)
            {
                subUnits.Add(subUnit.Name);
                subUnits.AddRange(GetAllSubUnitsRecursive(subUnit));
            }

            return subUnits;
        }

        public async Task<string> GetUnitIdAsync(string unitName)
        {
            return await _unitRepository.FindIdByNameAsync(unitName);
        }

        public IEnumerable<FindUnitDto> GetAllUnits()
        {
            var units = _unitRepository.GetAllUnits();

            // DTO'ya dönüştür
            var unitDtos = units.Select(u => new FindUnitDto
            {
                UnitId = u.UnitId,
                Name = u.Name,
                ParentName = u.ParentUnit != null ? u.ParentUnit.Name : null // Üst birim adı
            });

            return unitDtos;
        }


        public async Task<Unit> AddUnitAsync(UnitAddDto unitaddDto)
        {
            // DTO'dan Entity'ye manuel dönüştürme
            var unit = new Unit
            {
                UnitId = unitaddDto.UnitId,
                Name = unitaddDto.Name,
                ParentUnitId = unitaddDto.ParentUnitId,
                RoleId = unitaddDto.RoleId
            };

            return await _unitRepository.AddUnitAsync(unit);
        }

        public UnitTreeDto GetUnitTree(string name)
        {
            var rootUnit = _unitRepository.GetByNameA(name);
            {
                throw new KeyNotFoundException("Birim bulunamadı.");
            }

            return BuildUnitTree(rootUnit);
        }

        private UnitTreeDto BuildUnitTree(Unit unit)
        {
            var treeNode = new UnitTreeDto
            {
                UnitName = unit.Name,
                Children = unit.SubUnits?.Select(subUnit => BuildUnitTree(subUnit)).ToList()
            };

            return treeNode;
        }


        ////unit oluşturma
        //public async Task<CreateUnitDto> CreateUnitAsync(CreateUnitDto createUnitDto)
        //{
        //    var unitId = Guid.NewGuid().ToString();
        //    // DTO'dan Unit'e manuel dönüşüm
        //    var unit = new Unit
        //    {
        //        UnitId = unitId,
        //        Name = createUnitDto.Name,
        //        RoleId = createUnitDto.RoleId,
        //        ParentUnitId = createUnitDto.ParentUnitId
        //    };

        //    // Veritabanına ekle
        //    await _unitRepository.AddAsync(unit);
        //    await _unitRepository.SaveAsync();
        //    // Unit'ten UnitDto'ya manuel dönüşüm
        //    //var unitDto = new UnitDto
        //    //{
        //    //    Id = unit.Id,
        //    //    Name = unit.Name,
        //    //    RoleId = unit.RoleId,
        //    //    ParentId = unit.ParentId
        //    //};

        //    return createUnitDto;
        //}



        //public async Task<IEnumerable<UnitDto>> GetAllUnitsAsync()
        //{
        //    var units = await _unitRepository.GetAllAsync();
        //    return _mapper.Map<IEnumerable<UnitDto>>(units);
        //}

        //public async Task<UnitDto> GetUnitByIdAsync(int id)
        //{
        //    var unit = await _unitRepository.GetByIdAsync(id);
        //    return _mapper.Map<UnitDto>(unit);
        //}





        //public IEnumerable<UnitDto> GetAllUnits()
        //{
        //    var units = _unitRepository.GetAll();

        //    // Manuel dönüşüm: Unit'ten UnitDto'ya
        //    var unitDtos = units.Select(unit => new UnitDto
        //    {
        //        Id = unit.UnitId,
        //        UnitName = unit.Name,

        //    });

        //    // Log or debug to check if unitDtos is empty
        //    if (unitDtos == null || !unitDtos.Any())
        //    {
        //        Console.WriteLine("Mapped UnitDto list is empty.");
        //    }

        //    return unitDtos;
        //}


        //public UnitDto GetUnitById(int id)
        //{
        //    var unit = _unitRepository.GetById(id);
        //    return _mapper.Map<UnitDto>(unit);
        //}

        //public void CreateUnit(UnitDto unitDto)
        //{
        //    var unit = _mapper.Map<Unit>(unitDto);
        //    _unitRepository.Add(unit);
        //} 

        //public void UpdateUnit(UnitDto unitDto)
        //{
        //    var unit = _mapper.Map<Unit>(unitDto);
        //    _unitRepository.Update(unit);
        //}

        //public void DeleteUnit(int id)
        //{
        //    _unitRepository.Delete(id);
        //}



    }
}
