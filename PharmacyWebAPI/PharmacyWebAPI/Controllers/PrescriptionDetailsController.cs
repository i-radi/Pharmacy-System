/*using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmacyWebAPI.DataAccess.Repository.IRepository;
using PharmacyWebAPI.Models;
using PharmacyWebAPI.Models.ViewModels;
using System.Diagnostics;

namespace PharmacyWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public OrderDetailsController(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get(int id)
        {
            var obj = await  _unitOfWork.OrderDetails.GetAsync(id);
            if(obj is null)
            return BadRequest("Not Found");
            return Ok(obj);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll(int PID)
        {
            IEnumerable<OrderDetails> obj = await _unitOfWork.OrderDetails.GetAllFilterAsync(f => f.OrderId == PID);
            return Ok(obj);
        }

        [HttpPost]
        [Route("QuickCreate/{PID}")]
        public async Task<IActionResult> QuickCreate()
        {
            await _unitOfWork.OrderDetails.AddAsync(new OrderDetails
            {
                OrderId = _unitOfWork.Order.GetFirstOrDefaultAsync().Id,
            });
            _unitOfWork.Save();
            return Ok(new { success = true, message = "Product Created Successfully" });
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return Ok(new OrderDetailsDto());
        }

        //POST
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(OrderDetailsDto viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(viewModel);
            }
            await _unitOfWork.OrderDetails.AddAsync(_mapper.Map<OrderDetails>(viewModel));
            _unitOfWork.Save();
            return Ok(viewModel);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await _unitOfWork.OrderDetails.GetAsync(id);
            if (obj == null)
                return BadRequest(new { success = false, message = "Error While Deleting" });

            _unitOfWork.OrderDetails.Delete(obj);
            _unitOfWork.Save();

            return Ok(new { success = true, message = "OrderDetails Deleted Successfully" });
        }

        //POST
        [HttpPost]
        [Route("Edit")]
        public IActionResult Edit(OrderDetailsDto obj)
        {
            if (ModelState.IsValid)
            {
                return BadRequest(obj);
            }

            _unitOfWork.OrderDetails.Update(_mapper.Map<OrderDetails>(obj));
            _unitOfWork.Save();
            return Ok("OrderDetails updated successfully");
        }
    }
}*/