using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using user_service.Src.DTOs;
using user_service.Src.Models;

namespace user_service.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;


        public UserController(DataContext context , IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        /// <summary>
        /// Se realiza la petición de login
        /// </summary>
        /// <param name="loginDto"> Este Dto contiene solo el email y contraseña del usuario</param>
        /// <returns> La información del usuario</returns> 
        /// <summary>
     
        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if(user == null)
            {
                return BadRequest("Usuario inexistente, contacte a un administrador");
            }
            if(user.Role.Name == "Estudiante")
            {
                return BadRequest("Los estudiantes no pueden iniciar sesión");
            }

            if(!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return BadRequest("Credenciales incorrectas");
            }
            

            var response = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.Name
            };

            return Ok(response);
        }
        
        /// <summary>
        /// Se realiza la creación del estudiante
        /// </summary>
        /// <param name="createUserDto"></param>
        /// <returns></returns>
        /// <summary>
    

        [HttpPost("CreateStudent")]
        public async Task<ActionResult<User>> CreateStudent(CreateUserDto createUserDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == createUserDto.Email);
            if (existingUser != null)
            {
                return BadRequest("El estudiante ya existe");
            }
            if(!ValidEmailStudent(createUserDto.Email) || ValidEmailDocent(createUserDto.Email))
            {
                return BadRequest("El correo ingresado no es válido");
            }

            var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Estudiante");
            if (studentRole == null)
            {
                return BadRequest("El rol 'Estudiante' no existe");
            }
            var newStudent = new User
            {
                Id = Guid.NewGuid(),
                Name = createUserDto.Name,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Name),
                Role = studentRole
            };
            _context.Users.Add(newStudent);
            await _context.SaveChangesAsync();

            var response = new UserDto
            {
                Id = newStudent.Id,
                Name = newStudent.Name,
                LastName = newStudent.LastName,
                Email = newStudent.Email,
                Role = newStudent.Role.Name
            };

            return Ok(response);

        }

        //Función para crear un docente
        [HttpPost("CreateDocent")]
        public async Task<ActionResult<User>> CreateDocent(CreateUserDto createUserDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == createUserDto.Email);
            if (existingUser != null)
            {
                return BadRequest("El docente ya existe");
            }
            if(!ValidEmailDocent(createUserDto.Email) || ValidEmailStudent(createUserDto.Email))
            {   
                return BadRequest("El correo ingresado no es válido");
            }
            var docentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Docente");
            if (docentRole == null)
            {
                return BadRequest("El rol 'Docente' no existe");
            }

            var newDocent = new User
            {
                Id = Guid.NewGuid(),
                Name = createUserDto.Name,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Name),
                Role = docentRole
                    
            };
            _context.Users.Add(newDocent);
            await _context.SaveChangesAsync();

            var response = new UserDto
            {
                Id = newDocent.Id,
                Name = newDocent.Name,
                LastName = newDocent.LastName,
                Email = newDocent.Email,
                Role = newDocent.Role.Name
            };
            return Ok(response);

        }

        //Función para actualizar un estudiante
        [HttpPut("UpdateStudent/{id}")]
        public async Task<ActionResult<User>> UpdateStudent(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound("El estudiante no existe");
            }
            if(!ValidEmailStudent(user.Email))
            {
                return BadRequest("El correo no es válido");
            }
            if(user.Role.Id == 2)
            {
                return BadRequest("Los docentes no pueden ser actualizados");
            }

            user.Name = string.IsNullOrWhiteSpace(updateUserDto.Name) ? user.Name : updateUserDto.Name;
            user.LastName = string.IsNullOrWhiteSpace(updateUserDto.LastName) ? user.LastName : updateUserDto.LastName;
            user.Email = string.IsNullOrWhiteSpace(updateUserDto.Email) ? user.Email : updateUserDto.Email;

            await _context.SaveChangesAsync();

            var response = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
            };
            return Ok(response);
        }
    
        //Función para actualizar un docente
        [HttpPut("UpdateDocent/{id}")]
        public async Task<ActionResult<User>> UpdateDocent(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.Include(u=>u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound("El docente no existe");
            }

            if(!ValidEmailDocent(user.Email))
            {
                return BadRequest("El correo no es válido");
            }
            if(user.Role.Id == 3)
            {
                return BadRequest("Los estudiantes no pueden ser actualizados");
            }

            user.Name = string.IsNullOrWhiteSpace(updateUserDto.Name) ? user.Name : updateUserDto.Name;
            user.LastName = string.IsNullOrWhiteSpace(updateUserDto.LastName) ? user.LastName : updateUserDto.LastName;
            user.Email = string.IsNullOrWhiteSpace(updateUserDto.Email) ? user.Email : updateUserDto.Email;

            await _context.SaveChangesAsync();

            var response = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
            };
            return Ok(response);
        }

        [HttpGet("CheckStudent/{id}")]
        public async Task<bool> CheckStudent(Guid id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id );
            if (user == null)
            {
                return false;
            }
            if(user.Role.Name != "Estudiante")
            {
                return false;
            }

            return true;
        }
        
        //Función para obtener los estudiantes
        [HttpGet("GetStudents")]
        public async Task<ActionResult<IEnumerable<User>>> GetStudent(Guid id)
        {
            var students = await _context.Users
                .Where(u => u.Role.Name == "Estudiante") 
                .Select(u => new { u.Id, u.Name, u.LastName, u.Email, u.Role }) 
                .ToListAsync();

            return Ok(students);
        }

        //Función para obtener los docentes
        [HttpGet("GetDocents")]
        public async Task<ActionResult<User>> GetDocent(Guid id)
        {
            var docents = await _context.Users
                .Where(u => u.Role.Name == "Docente") 
                .Select(u => new { u.Id, u.Name, u.LastName, u.Email, u.Role }) 
                .ToListAsync();

            return Ok(docents);
        }
        
        //Función para validar el correo
        public bool ValidEmailStudent (string email)
        {
            return Regex.IsMatch(email,@"^[^@]+@alumnos\.ucn\.cl$",RegexOptions.IgnoreCase);
        }

        public bool ValidEmailDocent (string email)
        {
            return Regex.IsMatch(email,@"^[^@]+@(ucn\.cl|ce\.ucn\.cl)$",RegexOptions.IgnoreCase);
        }


    }
}