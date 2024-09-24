using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace user_service.Src.DTOs
{
    public class UpdateUserDto
    {
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "El nombre debe contener solo letras")]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Los apellidos deben contener solo letras")]
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}