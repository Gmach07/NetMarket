using Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Data
{
    public class SeguridadDbContextData
    {
        public static async Task SeedUserAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole>roleManager)
        {
            if (!userManager.Users.Any())
            {
                var usuario = new Usuario
                {
                    Nombre = "German",
                    Apellido = "Machado",
                    UserName = "German",
                    Email = "gmachado72015@gmail.com",
                    Direccion = new Direccion
                    {
                        Calle = "La Flor",
                        Ciudad = "La Libertad",
                        CodigoPostal = "1201",
                        Departamento = "Comayagua"

                    }
                };
                await userManager.CreateAsync(usuario, "Dolores@27");
            }

            if (!roleManager.Roles.Any())
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
