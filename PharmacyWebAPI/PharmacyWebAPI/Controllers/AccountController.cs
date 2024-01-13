using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PharmacyWebAPI.Models.Dto;
using PharmacyWebAPI.Utility.Services;
using PharmacyWebAPI.Utility.Services.IServices;
using System.Security.Cryptography;
using System.Text;

namespace PharmacyWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ISendGridEmail _sendGridEmail;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ISendGridEmail sendGridEmail, ITokenService tokenService)
        {
            _userManager = userManager;
            _sendGridEmail = sendGridEmail;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult GetRegister()
        {
            RegisterDto user = new();
            return Ok(new { User = user });
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tokenService.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        /*[HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDto userDto)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = userDto.Email,
                    Email = userDto.Email,
                };
                var result = await _userManager.CreateAsync(user, userDto.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok(new { success = true, message = "User Registered Successfully  ", User = userDto });
                }
            }
            return BadRequest(new { success = false, message = " Registered Faild  ", User = userDto });
        }*/

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> GetTokenAsync([FromForm] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tokenService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        /*
                [HttpGet]
                [Route("Login")]
                public IActionResult GetLogin()
                {
                    LoginDto user = new();
                    return Ok(new { User = user });
                }

                [HttpPost]
                [Route("Login")]
                public async Task<IActionResult> Login(LoginDto userDto)
                {
                    if (ModelState.IsValid)
                    {
                        var result = await _signInManager.PasswordSignInAsync(userDto.Email, userDto.Password, true, false);
                        if (result.Succeeded)
                            return Ok(new { success = true, message = "User Login Successfully  ", User = userDto });
                        if (result.IsLockedOut)
                            return BadRequest("Account Locked Out");
                    }
                    return BadRequest(new { success = false, message = " Login Faild  ", User = userDto });
                }*/

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return Ok(new { ForgotPassword = new ForgotPasswordDto() });
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is null)
                {
                    return NotFound(new { success = false, message = "NotFound" });
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackurl = Url.Action("ResetPassword", "Account", values: new { userId = user.Id, Code = code }, protocol: Request.Scheme);

                await _sendGridEmail.SendEmailAsync(model.Email, "Reset Email Confirmation", "Please reset email by going to this " +
                    "<a href=\"" + callbackurl + "\">link</a>");
                return Ok();
            }
            return BadRequest(model);
        }

        [HttpGet]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(string Code)
        {
            return Code is null ? BadRequest() : Ok();
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is null)
                    return BadRequest("Email Not Found");

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest("Invalid Token");
            }
            return BadRequest(model);
        }

        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] UserRolesDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tokenService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        [HttpGet("Decrypt")]
        public IActionResult Decrypt(string cipheredtextString)
        {
            try
            {
                string x = "Bq8KD/J98BU6CRrTSjem6Q==";
                byte[] key = Encoding.UTF8.GetBytes(x);
                Array.Resize(ref key, 16);

                string y = "EnvvZa61Min/2zSVMVno+w==";
                byte[] iv = Encoding.UTF8.GetBytes(y);
                Array.Resize(ref iv, 16);
                byte[] cipheredtext = Convert.FromBase64String(cipheredtextString);

                var simpletext = Decrypt(cipheredtext, key, iv);
                return Ok(simpletext);
            }
            catch (Exception ex)
            {
                return BadRequest("Wrong Key");
            }
        }

        private string Decrypt(byte[] cipheredtext, byte[] key, byte[] iv)
        {
            string simpletext = String.Empty;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
                using (MemoryStream memoryStream = new MemoryStream(cipheredtext))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            simpletext = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            return simpletext;
        }
    }
}