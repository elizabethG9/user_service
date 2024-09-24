using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using user_service.Src.Models;



namespace user_service.Src.Data
{
    public class DataSeeder
    {
        public void Seed(IServiceProvider serviceProvider)
        {
            using (var context = new DataContext(serviceProvider.GetRequiredService<DbContextOptions<DataContext>>()))
            {
                if (!context.Roles.Any())
                {
                    context.Roles.AddRange(
                        new Role { Name = "Admin" },
                        new Role { Name = "Docente" },
                        new Role { Name = "Estudiante" }
                    );
                    context.SaveChanges();
                }
                if (!context.Users.Any())
                {
                    var user = new User{
                        Name = "Guillermo",
                        LastName = "Gutierrez Pérez",
                        Email = "guillermo.gutierrez@ucn.cl",
                        Password = BCrypt.Net.BCrypt.HashPassword("Password123"),
                        RoleId = 1
                    };
                    context.Users.Add(user);
                    context.SaveChanges();
                }
            }
        }

        internal static void SeedData(IServiceProvider services)
        {
            var seeder = new DataSeeder();
            seeder.Seed(services);
        }
    }
}