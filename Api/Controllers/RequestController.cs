using Microsoft.AspNetCore.Mvc;
using Services.Abstract.ModelService;
using Entities.Dto;
using System.Threading.Tasks;
using Services.Abstract.Authentication;
using Microsoft.Extensions.Logging;
using Services.Concrete.ModelManager;
using Entities.Models;
using System;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly IAuthenticationService _service;
        private readonly IUserService _userService;
        private readonly ILogger<RequestController> _logger;
        private readonly IUnitService _unit;
        private readonly IPersonelService _personelService;
        private readonly IPersonelUnitService _personelUnitService;
        private readonly IPersonelRoleService _personelRoleService;
        private readonly IRolesService _roleService;



        public RequestController(IRequestService requestService, IAuthenticationService service, IUserService userService,
            ILogger<RequestController> logger, IPersonelService personelService,
            IPersonelUnitService personelUnitService,
            IPersonelRoleService personelRoleService,
           IRolesService roleService, IUnitService unit
           )
        {
            _requestService = requestService;
            _service = service;
            _userService = userService;
            _logger = logger;
            _personelService = personelService;
            _personelUnitService = personelUnitService;
            _personelRoleService = personelRoleService;
            _roleService = roleService;
            _unit = unit;
      
        }

        [HttpGet("Birim-Agaci")]
        public IActionResult GetUnitTree(string name)
        {
            try
            {
                var unitTree = _unit.GetUnitTree(name);
                return Ok(new { Success = true, Result = unitTree });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Bir hata oluştu.", Error = ex.Message });
            }
        }

        //kullanıcı kaydı
      
        [HttpPost("Kullanici-Kaydi")]
      

        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistrationDto)
        {
            var result = await _service.RegisterUser(userForRegistrationDto); // Kullanıcı kaydı işlemi

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return StatusCode(201);
        }
        //kullanıcı girişi
        [HttpPost("login")]

        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            if (!await _service.ValidatePersonel(user))  // Kullanıcı doğrulama işlemi
                return Unauthorized(); // 401 Unauthorized

            var tokenDto = await _service.CreateToken(populateExp: true); // Token oluşturma işlemi

            return Ok(tokenDto);
        }



        [HttpPost("refresh")]

        public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
        {
            var tokenDtoToReturn = await _service.RefreshToken(tokenDto); // Token yenileme işlemi
            return Ok(tokenDtoToReturn);
        }


        [HttpGet("Personel-Is-Giris-Cikis")]
        //[Authorize(Roles = "Personel")]
        public async Task<IActionResult> GetPersonelStatus(string personelId)
        {
            var status = await _personelService.GetPersonelStatusAsync(personelId);
            return Ok(status);
        }

        [HttpGet("Tum-Personel-Is-Giris-Cikis")]
        public async Task<IActionResult> GetAllPersonelStatuses()
        {
            var statuses = await _personelService.GetAllPersonelStatusesAsync();
            return Ok(statuses);
        }


        [HttpPost("Unit-Ekle")]
        public async Task<IActionResult> AddUnit([FromBody] UnitAddDto unitaddDto)
        {
            if (unitaddDto == null)
                return BadRequest("Unit data cannot be null.");

            var result = await _unit.AddUnitAsync(unitaddDto);
            return Ok(result);
        }

        [HttpGet("FindPersonelId")]
        public IActionResult FindPersonelId(string firstName, string lastName)
        {
            var personelIdDto = _personelService.FindPersonelId(firstName, lastName);

            if (personelIdDto == null)
            {
                return NotFound("Personel bulunamadı.");
            }

            return Ok(personelIdDto);
        }

        [HttpGet("GetAllUnits")]
        public IActionResult GetAllUnits()
        {
            try
            {
                var units = _unit.GetAllUnits();
                if (units == null || !units.Any())
                {
                    return NotFound(new { Success = false, Message = "Hiçbir birim bulunamadı." });
                }
                return Ok(new { Success = true, Result = units });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Bir hata oluştu.", Error = ex.Message });
            }
        }
        [HttpPost("Isten-Cikma")]
        public async Task<IActionResult> TerminatePersonel( string personelId)
        {
            var result = await _personelService.TerminatePersonelAsync(personelId);
            if (result == "Personel bulunamadı.")
            {
                return NotFound(result);
            }

            return Ok(result);
        }


        [HttpPost("Personel-Atama")]
        public IActionResult AssignUnitToPersonel([FromBody] PersonelUnitDto personelUnitDto)
        {
            try
            {
                _personelService.AssignPersonelToUnit(personelUnitDto.PersonelId, personelUnitDto.UnitId);
                return Ok("Personel ve birim başarıyla ilişkilendirildi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "İşlem sırasında bir hata oluştu.", exception = ex.Message });
            }
        }

        [HttpGet("FindUnitId")]
        public async Task<IActionResult> FindUnitId([FromQuery] string unitName)
        {
            if (string.IsNullOrEmpty(unitName))
            {
                return BadRequest("Unit name cannot be empty.");
            }

            var unitId = await _unit.GetUnitIdAsync(unitName);

            if (string.IsNullOrEmpty(unitId))
            {
                return NotFound("Unit not found.");
            }

            var response = new FindUnitDto
            {
                UnitId = unitId,
                Name = unitName,

            };

            return Ok(response);
        }



        [HttpPost("Rol-Ekle")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> AddRole([FromBody] RoleDto roleDto)
        {
            try
            {
                await _roleService.AddRoleAsync(roleDto);
                return Ok("Rol başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }



        [HttpPost("Rol-Atama")]
        //[Authorize(Roles = "Müdürlük")]
        public async Task<IActionResult> AssignRoleToPersonel([FromBody] PersonelRoleDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            var result = await _personelRoleService.AssignRoleToPersonel(dto);

            if (!result)
            {
                return NotFound("Personel or Role not found.");
            }

            return Ok("Role assigned successfully.");
        }

        [HttpGet("GetAllRoles")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            var roleDtos = roles.Select(role => new RoleDto
            {
                RoleId=role.RoleId,
                Name = role.Name,
                Description = role.Description
            }).ToList();

            return Ok(roleDtos);
        }




        [HttpPost("TalepOlustur")]
        public IActionResult CreateRequest([FromBody] RequestPersonelDto requestPersonelDto)
        {
            try
            {
                _requestService.CreateRequest(requestPersonelDto); // Asenkron yerine senkron metodu çağırıyoruz
                return Ok("Talep başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bir hata oluştu: {ex.Message}");
            }
        }

        [HttpPost("Talep-Yonlendirme")]
        //[Authorize]
        public IActionResult RedirectRequest([FromBody] RedirectRequestDto redirectRequestDto)
        {
            try
            {
                _requestService.RedirectRequest(redirectRequestDto);
                return Ok("Talep başarıyla yönlendirildi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Talep-Onaylama")]
        //[Authorize]
        public IActionResult UpdateRequestStatus([FromBody] UpdateRequestStatusDto updateRequestStatusDto)
        {
            try
            {
                _requestService.UpdateRequestStatus(updateRequestStatusDto);
                return Ok("Talep durumu güncellendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("Personel-Talep-Goruntuleme")]
        public IActionResult GetRequestsByPersonel(string personelId)
        {
            try
            {
                // Personel ID'ye ait talepleri al
                var requests = _requestService.GetRequestsByUser(personelId); // Asenkron yerine senkron metodu çağırıyoruz

                // Başarılı bir şekilde talepler döndürülür
                return Ok(requests);
            }
            catch (Exception ex)
            {
                // Hata durumunda 500 Internal Server Error döndürülür
                return StatusCode(500, $"Hata: {ex.Message}");
            }
        }


        [HttpGet("Talep-Durum-Goruntuleme")]
        public IActionResult GetRequestStatus(string requestId)
        {
            try
            {
                // En son işlem kaydını veritabanından alıyoruz
                var statusDescription = _requestService.GetRequestProcessStatusDescription(requestId);

                // Eğer başarıyla alındıysa, statusDescription'ı döndürüyoruz
                return Ok(new { StatusDescription = statusDescription.ToString() });
            }
            catch (KeyNotFoundException ex)
            {
                // Eğer talep bulunamazsa 404 döndürülür
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Genel hatalar için 500 döndürülür
                return StatusCode(500, new { Message = "Bir hata oluştu.", Error = ex.Message });
            }
        }


        [HttpPatch("Birim-Tasima")]
        public async Task<IActionResult> ChangeUnitParent([FromBody] MoveUnitWithChildrenDto dto)
        {
            try
            {
                var result = await _unit.ChangeUnitParentAsync(dto);
                return result ? Ok() : BadRequest("Invalid unit or department ID.");
            }

            catch (KeyNotFoundException ex)
            {
                // Eğer talep bulunamazsa 404 döndürülür
                return NotFound(new { Message = ex.Message });
            }

        }
        [HttpGet("Ust-Birim-Alt-Birim-Listeleme")]
        public IActionResult GetUnits(string unitName = null, string firstName = null, string lastName = null)
        {
            try
            {
                // Eğer unitName verilmişse, birim adı ile ilgili birimleri al
                if (!string.IsNullOrEmpty(unitName))
                {
                    var unitDtos = _unit.GetUnitsByUnitName(unitName);
                    if (unitDtos == null || !unitDtos.Any())
                    {
                        return NotFound(new { Success = false, Message = "Birim bulunamadı." });
                    }
                    return Ok(new { Success = true, Result = unitDtos });
                }
                // Eğer firstName ve lastName verilmişse, personel ismi ile ilgili birimleri al
                else if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                {
                    var units = _unit.GetUnitsByPersonel(firstName, lastName);
                    if (units == null || !units.Any())
                    {
                        return NotFound(new { Success = false, Message = "Personel bulunamadı." });
                    }
                    return Ok(new { Success = true, Result = units });
                }
                // Eğer hiç parametre verilmemişse, hata döndür
                else
                {
                    return BadRequest(new { Success = false, Message = "Birim adı veya personel ismi sağlanmalıdır." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Bir hata oluştu.", Error = ex.Message });
            }
        }

      




        [HttpGet("Personel-Listele")]
        public async Task<IActionResult> GetAllPersonel([FromQuery] bool onlyActive = false)
        {
            var personels = await _personelService.GetAllPersonelsAsync(onlyActive);
            return Ok(personels); // Personel DTO'larını döndürüyoruz
        }

       
    }
}









        //[HttpGet("Birim-Listele")]
        //public async Task<IActionResult> GetAll()
        //{
        //    var units = await _unit.GetAllUnitsAsync();
        //    return Ok(units);
        //}

//[HttpGet("Birim-Listele")]
//public IActionResult GetUnits([FromQuery] string firstName, [FromQuery] string lastName, [FromQuery] string unitName)
//{
//    if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(unitName))
//    {
//        return BadRequest("Lütfen personel adı, soyadı veya birim adı girin.");
//    }

//    IEnumerable<UnitWithParentChildDto> units = Enumerable.Empty<UnitWithParentChildDto>();

//    // Personel adı ve soyadı ile birimleri alıyoruz
//    if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
//    {
//        units = _unit.GetUnitsByPersonel(firstName, lastName);  // Asenkron yerine senkron çağırıyoruz
//    }

//    // Birim adı ile birimleri alıyoruz
//    if (!string.IsNullOrWhiteSpace(unitName))
//    {
//        units = _unit.GetUnitsByUnitName(unitName);  // Asenkron yerine senkron çağırıyoruz
//    }

//    if (!units.Any())
//    {
//        return NotFound("Hiçbir birim bulunamadı.");
//    }

//    return Ok(units);
//}

//[HttpGet("hierarchy")]
//public IActionResult GetUnitHierarchy(string? firstName, string? lastName, string? unitName)
//{
//    try
//    {
//        if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
//        {
//            // Personel ismine göre işlem yap
//            var result = _unit.GetUnitsByPersonel(firstName, lastName);
//            return Ok(result);
//        }
//        else if (!string.IsNullOrEmpty(unitName))
//        {
//            // Birim adına göre işlem yap
//            var result = _unit.GetUnitsByUnitName(unitName);
//            return Ok(result);
//        }
//        else
//        {
//            return BadRequest(new { Message = "Lütfen ya personel ismi (firstName, lastName) ya da birim adı (unitName) sağlayın." });
//        }
//    }
//    catch (KeyNotFoundException ex)
//    {
//        return NotFound(new { Message = ex.Message });
//    }
//    catch (Exception ex)
//    {
//        return StatusCode(500, new { Message = "Bir hata oluştu.", Error = ex.Message });
//    }
//}

//[HttpGet("GetUnitsByUnitName")]
//public IActionResult GetUnitsByUnitName(string unitName)
//{
//    try
//    {
//        // İlgili servisi çağırarak birim bilgilerini alıyoruz
//        var unitDtos = _unit.GetUnitsByUnitName(unitName);
//        if (unitDtos == null || !unitDtos.Any())
//        {
//            return NotFound(new { Message = "Birim bulunamadı." });
//        }

//        // Başarılı ise 200 OK döndür
//        return Ok(unitDtos);
//    }
//    catch (Exception ex)
//    {
//        // Hata durumunda 500 döndür
//        return StatusCode(500, new { Message = "Bir hata oluştu.", Error = ex.Message });
//    }
//}



//[HttpGet("units-by-personel")]
//public IActionResult GetUnitsByPersonelName(string firstName, string lastName)
//{
//    try
//    {
//        var units = _unit.GetUnitsByPersonel(firstName, lastName);
//        return Ok(units);
//    }
//    catch (KeyNotFoundException ex)
//    {
//        return NotFound(new { Message = ex.Message });
//    }
//    catch (Exception ex)
//    {
//        return StatusCode(500, new { Message = "Bir hata oluştu.", Error = ex.Message });
//    }
//}



//[HttpGet("Unit-Görüntüleme")]
//public IActionResult GetAllUnits()
//{
//    var units = _unit.GetAllUnits();
//    return Ok(units);
//}


//[HttpGet("{id}")]
//public IActionResult GetById(int id)
//{
//    var unit = _unit.GetUnitById(id);
//    if (unit == null)
//    {
//        return NotFound();
//    }
//    return Ok(unit);
//}

//[HttpPost("Unit-Ekleme")]
//public IActionResult Create([FromBody] UnitDto unitDto)
//{
//    if (!ModelState.IsValid)
//    {
//        return BadRequest(ModelState);
//    }

//    _unit.CreateUnit(unitDto);

//    // Sadece 201 durum kodu ve eklenen veriyi döndür
//    return StatusCode(201, unitDto);
//}

//[HttpPut("Unit-Güncelleme")]
//public IActionResult Update(string id, [FromBody] UnitDto unitDto)
//{
//    if (id != unitDto.Id)
//    {
//        return BadRequest();
//    }

//    _unit.UpdateUnit(unitDto);
//    return NoContent();
//}

//[HttpDelete("Unit-Silme")]
//public IActionResult Delete(int id)
//{
//    _unit.DeleteUnit(id);
//    return NoContent();
//}





