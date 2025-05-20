using Entities.EntityType.Abstract;
using Entities.EntityType.Concrete;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Roles 
    {
        public string RoleId { get; set; }
       
        public string RoleName { get; set; } 
        public string? Description { get; set; } // Görev açıklaması

        // public ICollection<Unit> Units { get; set; }
        public List<Unit> Units { get; set; }
        public ICollection<PersonelRoles> PersonelRoles { get; set; } // Ara tablo ilişkisi

    }
}
