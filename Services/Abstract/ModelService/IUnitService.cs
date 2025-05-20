using Entities.Dto;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstract.ModelService
{
    public interface IUnitService
    {
        Task<bool> ChangeUnitParentAsync(MoveUnitWithChildrenDto dto);
        IEnumerable<UnitWithParentChildDto> GetUnitsByPersonel(string firstName, string lastName);
        IEnumerable<UnitWithParentChildDto> GetUnitsByUnitName(string unitName);
        Task<string> GetUnitIdAsync(string unitName);
        IEnumerable<FindUnitDto> GetAllUnits();

        UnitTreeDto GetUnitTree(string name);
        Task<Unit> AddUnitAsync(UnitAddDto unitaddDto);
      
        //Task<CreateUnitDto> CreateUnitAsync(CreateUnitDto createUnitDto);

        //Task<IEnumerable<UnitDto>> GetAllUnitsAsync();
        //Task<UnitDto> GetUnitByIdAsync(int id);






        //IEnumerable<UnitDto> GetAllUnits();
        //UnitDto GetUnitById(int id);
        //void CreateUnit(UnitDto unitDto);
        //void UpdateUnit(UnitDto unitDto);
        //void DeleteUnit(int id);
        //Task<IEnumerable<UnitDto>> GetAllUnitsAsync();
        //UnitWithParentChildDto GetUnitsByPersonelName(string firstName, string lastName);
        //UnitWithParentChildDto GetUnitsByUnitName(string unitName);
        // Birim adı ile birimleri almak için senkron metod

    }
}
