﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto
{
    public class UserForAuthenticationDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; init; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; init; }
        public bool IsPersonel { get; set; }  // Personel olup olmadığını belirleyen özellik
    }
}
