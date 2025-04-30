using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(Usuario usuario);
       /* string CreateRefreshToken(string username, string email, string role, string id);
        bool ValidateToken(string token);
        bool ValidateRefreshToken(string refreshToken);
        string GetUsernameFromToken(string token);
        string GetEmailFromToken(string token);
        string GetRoleFromToken(string token);
        string GetIdFromToken(string token);*/
    }
}
