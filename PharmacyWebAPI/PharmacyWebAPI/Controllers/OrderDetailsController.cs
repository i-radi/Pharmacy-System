/*using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmacyWebAPI.DataAccess.Repository.IRepository;
using PharmacyWebAPI.Models;
using PharmacyWebAPI.Models.Dto;
using System.Diagnostics;

namespace PharmacyWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionDetailsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public PrescriptionDetailsController(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get(int id)
        {
            var obj = await _unitOfWork.PrescriptionDetails.GetAsync(id);
            if (obj is null)
                return BadRequest("Not Found");
            return Ok(obj);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll(int PID)
        {
            IEnumerable<PrescriptionDetails> obj = await _unitOfWork.PrescriptionDetails.GetAllFilterAsync(f => f.PrescriptionId == PID);
            return Ok(obj);
        }

        [HttpPost]
        [Route("QuickCreate/{PID}")]
        public async Task<IActionResult> QuickCreate()
        {
            await _unitOfWork.PrescriptionDetails.AddAsync(new PrescriptionDetails
            {
                PrescriptionId = _unitOfWork.Prescription.GetFirstOrDefaultAsync().Id,
            });
            _unitOfWork.SaveAsynce();
            return Ok(new { success = true, message = "Product Created Successfully" });
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return Ok(new PrescriptionDetailsDto());
        }

        //POST
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(PrescriptionDetailsDto viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(viewModel);
            }
            await _unitOfWork.PrescriptionDetails.AddAsync(_mapper.Map<PrescriptionDetails>(viewModel));
            _unitOfWork.SaveAsynce();
            return Ok(viewModel);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await _unitOfWork.PrescriptionDetails.GetAsync(id);
            if (obj == null)
                return BadRequest(new { success = false, message = "Error While Deleting" });

            _unitOfWork.PrescriptionDetails.Delete(obj);
            _unitOfWork.SaveAsynce();

            return Ok(new { success = true, message = "PrescriptionDetails Deleted Successfully" });
        }

        //POST
        [HttpPost]
        [Route("Edit")]
        public IActionResult Edit(PrescriptionDetailsDto obj)
        {
            if (ModelState.IsValid)
            {
                return BadRequest(obj);
            }

            _unitOfWork.PrescriptionDetails.Update(_mapper.Map<PrescriptionDetails>(obj));
            _unitOfWork.SaveAsynce();
            return Ok("PrescriptionDetails updated successfully");
        }
    }
}*/