using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class UsuarioSpecification : BaseSpecification<Usuario>
    {
        public UsuarioSpecification(UsuarioSpecificationParams usuarioParams)
            : base(x => 
                (string.IsNullOrEmpty(usuarioParams.Search) || x.Nombre.ToLower().Contains(usuarioParams.Search) || x.Apellido.ToLower().Contains(usuarioParams.Search)) &&
                (string.IsNullOrEmpty(usuarioParams.Nombre) || x.Nombre.ToLower().Contains(usuarioParams.Nombre.ToLower())) &&
                (string.IsNullOrEmpty(usuarioParams.Apellido) || x.Apellido.ToLower().Contains(usuarioParams.Apellido.ToLower())))
        {
            AddOrderBy(x => x.UserName);
            ApplyPaging((usuarioParams.PageIndex - 1) * usuarioParams.PageSize, usuarioParams.PageSize);
            if (!string.IsNullOrEmpty(usuarioParams.Sort))
            {
                switch (usuarioParams.Sort)
                {
                    case "nombreAsc":
                        AddOrderBy(x => x.Nombre);
                        break;
                    case "nombreDesc":
                        AddOrderByDescending(x => x.Nombre);
                        break;
                    case "apellidoAsc":
                        AddOrderBy(x => x.Apellido);
                        break;
                    case "apellidoDesc":
                        AddOrderByDescending(x => x.Apellido);
                        break;
                    case "emailAsc":
                        AddOrderBy(x => x.Email);
                        break;
                    case "emailDesc":
                        AddOrderByDescending(x => x.Email);
                        break;
                    default:
                        AddOrderBy(x => x.UserName);
                        break;
                }
            }
        }
    }
}
