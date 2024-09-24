using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace user_service.Src.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Genera un UUID v4 automáticamente
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}