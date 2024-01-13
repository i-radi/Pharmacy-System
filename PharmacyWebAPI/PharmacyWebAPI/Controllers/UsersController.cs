using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyWebAPI.Models;
using PharmacyWebAPI.Utility.Services.IServices;

namespace PharmacyWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;

        public UsersController(UserManager<User> userManager, IUnitOfWork unitOfWork, IPhotoService photoService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
        }

        [HttpPost]
        [Route("AddPhoto/{id}")]
        public async Task<IActionResult> AddPhoto(string id, IFormFile file)
        {
            var drug = await _unitOfWork.User.GetFirstOrDefaultAsync(x => x.Id == id);
            if (drug == null)
                return NotFound();
            if (!string.IsNullOrEmpty(drug.ImageId))
            {
                var DeleteResult = await _photoService.DeletePhotoAsync(drug.ImageId);
                if (DeleteResult.Error is not null)
                    return BadRequest(DeleteResult.Error);
            }
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null)
                return BadRequest(result.Error);
            drug.ImageId = result.PublicId;
            drug.ImageURL = result.SecureUrl.AbsoluteUri;
            _unitOfWork.User.Update(drug);
            await _unitOfWork.SaveAsync();

            return Ok(drug.ImageURL);
        }
    }
}