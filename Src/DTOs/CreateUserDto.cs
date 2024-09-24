using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace user_service.Src.DTOs
{
    public class CreateUserDto
    {
       
        [Required(ErrorMessage = "El nombre es requerido")] 
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Los apellidos deben contener solo letras")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "El apellido es requerido")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Los apellidos deben contener solo letras")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "El email es requerido")]
        public string Email { get; set; } = string.Empty;
    }
}