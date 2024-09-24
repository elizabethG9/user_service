using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace user_service.Src.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El email es requerido")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;
    }
}