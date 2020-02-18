using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ddacAPI.Data;
using ddacAPI.Models;
using ddacAPI.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Web;
using System.IO;

namespace ddacAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ddacAPIContext _context;
        private static BlobManager _blobManager;

        public IConfiguration Configuration { get; }

        public CustomerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ddacAPIContext context, IConfiguration configuration)
        {
            Configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            
        }

        [HttpPost]
        [Route("signup")]
        //POST : /api/Customer/signup
        public async Task<Object> PostCustomer(UserAuthModel model)
        {
            model.Role = "Customer";
            var customerToSignup = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email,
                Customer = new Customer()
                {
                    Name = model.Username
                }

            };

            try
            {
                var result = await _userManager.CreateAsync(customerToSignup, model.Password);
                await _userManager.AddToRoleAsync(customerToSignup, model.Role);
                return Ok(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        [Route("login")]
        //POST: /api/customer/login
        public async Task<IActionResult> Login(UserAuthModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                //Get role assigned to the user
                var role = await _userManager.GetRolesAsync(user);
                IdentityOptions _options = new IdentityOptions();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim(_options.ClaimsIdentity.RoleClaimType,role.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddHours(3),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456")), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
                return BadRequest(new { message = "Email or password is incorrect." });
        }

        [HttpGet]
        [Authorize(Roles ="Customer")]
        [Route("profile")]
        //GET: /api/customer/profile
        public async Task<Object> GetUserProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var customer = await _context.Customer.FindAsync(userId);
            return new
            {
                user.Email,
                user.UserName,
                customer.Id,
                customer.Name,
                customer.ContactNumber,
                customer.ProfilePic
            };
        }

        [HttpPut]
        [Authorize(Roles = "Customer")]
        [Route("profile")]
        //GET: /api/customer/profile
        public async Task<IActionResult> EditUserProfile([FromBody] Customer customer)
        {
            

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [Route("upload")]
        //GET: /api/customer/upload
        public async Task<IActionResult> UploadUserProfilePic(IFormFile newFile)
        {
            _blobManager = new BlobManager(Configuration);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Claims.First(c => c.Type == "UserID").Value;

            var user = await _userManager.FindByIdAsync(userId);

            var customer = await _context.Customer.FindAsync(userId);
          

            if (newFile != null)
            {
                Stream filestreamFromRequest = newFile.OpenReadStream();
                customer.ProfilePic = await _blobManager.UploadFileToStorageAsync(filestreamFromRequest, "customer_"+userId.ToString() + ".jpg");
            } else
            {
                return BadRequest();
            }


            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();

        }
    }
}