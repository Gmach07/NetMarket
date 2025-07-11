using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class UsuarioForCountingSpecification : BaseSpecification<Usuario>
    {
        public UsuarioForCountingSpecification(UsuarioSpecificationParams usuarioParams)
            : base(x => 
                (string.IsNullOrEmpty(usuarioParams.Search) || x.Nombre.ToLower().Contains(usuarioParams.Search) || x.Apellido.ToLower().Contains(usuarioParams.Search)) &&
                (string.IsNullOrEmpty(usuarioParams.Nombre) || x.Nombre.ToLower().Contains(usuarioParams.Nombre.ToLower())) &&
                (string.IsNullOrEmpty(usuarioParams.Apellido) || x.Apellido.ToLower().Contains(usuarioParams.Apellido.ToLower())))
        {

        }

    }
}
