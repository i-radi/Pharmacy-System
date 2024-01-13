using PharmacyWebAPI.Models;
using PharmacyWebAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyWebAPI.Utility.Services.IServices
{
    public interface ITokenService
    {
        Task<AuthModelDto> RegisterAsync(RegisterDto model);

        Task<AuthModelDto> GetTokenAsync(LoginDto model);

        Task<string> AddRoleAsync(UserRolesDto model);

        string DataFromToken(string token, Func<Claim, bool> selector);
    }
}