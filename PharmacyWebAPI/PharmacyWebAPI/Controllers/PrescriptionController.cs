using AutoMapper;
using Newtonsoft.Json;
using PharmacyWebAPI.Models;
using PharmacyWebAPI.Models.Dto;
using PharmacyWebAPI.Utility.Services.IServices;
using Stripe.Checkout;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PharmacyWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrescriptionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public PrescriptionController(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var obj = await _unitOfWork.Prescription.GetFirstOrDefaultAsync(p => p.Id == id);
            if (obj is null)
                return BadRequest("Not Found");
            return Ok(obj);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

            string userId = _tokenService.DataFromToken(token, t => t.Type == "uid");

            if (userId == null)
                return Unauthorized();
            // Perform authorization check based on user ID
            var obj = await _unitOfWork.Prescription.GetAllFilterAsync(u => u.PatientId == userId, x => x.Patient, y => y.Doctor);
            if (obj is null)
                return NotFound();
            return Ok(obj);
        }

        [HttpGet]
        [Route("GetPrescriptionDetails/{id}")]
        public async Task<IActionResult> GetPrescriptionDetails(int id)
        {
            var obj = await _unitOfWork.PrescriptionDetails.GetAllFilterAsync(p => p.PrescriptionId == id, d => d.Drug);
            if (obj is null)
                return NotFound();
            return Ok(new { Order = obj });
        }

        [HttpPost]
        [Route("Create/{id}")]
        public async Task<IActionResult> Create(string id, IEnumerable<PrescriptionDetailsDto> Drugs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Drugs);
            }
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault().Replace("Bearer ", "");

            string userId = _tokenService.DataFromToken(token, t => t.Type == "uid");
            if (userId == null)
                return Unauthorized();
            var prescription = new Prescription() { DoctorId = "userId", PatientId = id, };
            await _unitOfWork.Prescription.AddAsync(prescription);
            await _unitOfWork.SaveAsync();
            var drugs = _mapper.Map<IEnumerable<PrescriptionDetails>>(Drugs).ToList();
            await _unitOfWork.PrescriptionDetails.SetPresciptionId(prescription.Id, drugs);
            return Ok(new { success = true, message = "Prescription Created Successfully", drugs });
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await _unitOfWork.Prescription.GetFirstOrDefaultAsync(p => p.Id == id); ;
            if (obj == null)
                return BadRequest(new { success = false, message = "Error While Deleting" });

            _unitOfWork.Prescription.Delete(obj);
            await _unitOfWork.SaveAsync();

            return Ok(new { success = true, message = "Prescription Deleted Successfully" });
        }

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(PrescriptionDto obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(obj);
            }

            _unitOfWork.Prescription.Update(_mapper.Map<Prescription>(obj));
            await _unitOfWork.SaveAsync();
            return Ok(obj);
        }

        [HttpPost]
        [Route("Dispensing/{id}")]
        public async Task<IActionResult> Dispensing(int id, [FromForm] ResponseURLsDto URLs)
        {
            string userId = _tokenService.DataFromToken(URLs.token, token => token.Type == "uid");

            if (userId == null)
                return Unauthorized();

            var prescriptionDetails = await _unitOfWork.PrescriptionDetails.GetAllFilterAsync(p => p.PrescriptionId == id);
            var orderDetails = _unitOfWork.PrescriptionDetails.PrescriptionDetailsToOrderDetails(prescriptionDetails.ToList());

            var order = new Order { UserId = userId };
            await _unitOfWork.Order.AddAsync(order);
            await _unitOfWork.SaveAsync();

            var presc = await _unitOfWork.Prescription.GetFirstOrDefaultAsync(p => p.Id == id);
            presc.OrderId = order.Id;
            await _unitOfWork.SaveAsync();

            order.OrderTotal = _unitOfWork.Order.GetTotalPrice(orderDetails);
            await _unitOfWork.OrderDetail.SetOrderId(order.Id, orderDetails);
            var session = await _unitOfWork.Order.StripeSetting(order, orderDetails, URLs);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [HttpGet]
        [Route("OrderConfirmation/{id}")]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(o => o.Id == id);
            var prescription = await _unitOfWork.Prescription.GetFirstOrDefaultAsync(p => p.OrderId == id);

            var service = new SessionService();
            Session session = service.Get(order.SessionId);
            //check the stripe status
            _unitOfWork.Order.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
            prescription.Dispensing = true;
            await _unitOfWork.SaveAsync();

            //_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Pharmacy App", "<p>New Order Created</p>");

            return Ok(new { success = true, message = "Order Confirm Successfully", OrderId = id });
        }

        [HttpGet]
        [Route("Denied/{id}")]
        public async Task<IActionResult> Denied(int id)
        {
            var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(o => o.Id == id);
            _unitOfWork.Order.Delete(order);
            await _unitOfWork.SaveAsync();
            return Ok(new { success = false, message = "Order Denied" });
        }
    }
}