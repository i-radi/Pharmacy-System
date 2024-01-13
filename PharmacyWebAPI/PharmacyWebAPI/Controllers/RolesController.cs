﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PharmacyWebAPI.DataAccess;
using PharmacyWebAPI.Models.Dto;

namespace PharmacyWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }

        /* [HttpPost]
         [Route("Seed")]
         public async Task<IActionResult> SeedRoles()
         {
             if (!_context.Roles.Any())
             {
                 await _roleManager.CreateAsync(new IdentityRole("Admin"));
                 await _roleManager.CreateAsync(new IdentityRole("User"));
                 return Ok();
             }
             return NoContent();
         }*/

        [HttpPost]
        public async Task<IActionResult> Add(RoleFormDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(await _roleManager.Roles.ToListAsync());

            if (await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("Name", "Role is exists!");
                return BadRequest(await _roleManager.Roles.ToListAsync());
            }

            await _roleManager.CreateAsync(new IdentityRole(model.Name.Trim()));

            return Ok();
        }
    }
}