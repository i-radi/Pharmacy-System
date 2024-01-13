/*using Microsoft.AspNetCore.Mvc;
using PharmacyWebAPI.DataAccess;
using PharmacyWebAPI.DataAccess.Repository.IRepository;
using PharmacyWebAPI.Models;
using System.Diagnostics;

namespace PharmacyWebAPI.Controllers
{
    public class ShopController : Controller
    {
        private readonly ILogger<ShopController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ShopController(ILogger<ShopController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return BadRequest(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}*/