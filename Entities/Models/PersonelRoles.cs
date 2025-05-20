using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class PersonelRoles 
    {
    
        public string PersonelId { get; set; } // Foreign Key to Personel
        public Personel Personel { get; set; } // Navigasyon özelliği

        public string RoleId { get; set; } // Foreign Key to Role
        public Roles Role { get; set; } // Navigasyon özelliği
    }
}
