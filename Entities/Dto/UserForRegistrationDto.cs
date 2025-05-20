using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public record UserForRegistrationDto
    {

        public string ? FirstName { get; init; }
        public string? LastName { get; init; }

        [Required(ErrorMessage = "Username is required.")]
        public string? UserName { get; init; }

        [Required(ErrorMessage = "Password is required.")]
        public string?   Password { get; init; }

        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        //public string Name { get; init; }
        //public string Surname { get; init; }
        //public string PersonelName { get; set; }
        //public string Unit { get; set; }
        public bool IsPersonel { get; set; }  // Personel olup olmadığını belirleyen özellik
        //public string PersonelId { get; set; }
        //public ICollection<string>? Roles { get; init; }

    }
}
