using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ddacAPI.Data;
using ddacAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ddacAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ddacAPIContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ddacAPIContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;

        }


        [HttpPost]
        [Route("login")]
        //POST: /api/admin/login
        public async Task<IActionResult> Login(UserAuthModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {

                //Get role assigned to the user
                //var role = await _userManager.GetRolesAsync(user);
                //if (role == null)
                //{
                //    await _userManager.AddToRoleAsync(user, "Admin");
                    //role = await _userManager.GetRolesAsync(user);
                //}
                IdentityOptions _options = new IdentityOptions();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim(_options.ClaimsIdentity.RoleClaimType, "Admin")
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

        //GET: /api/admin/partner
        [HttpGet("partner")]
        [Authorize(Roles = "Admin")]
        public IEnumerable<Partner> GetPartner()
        {
            return _context.Partner.Include(r => r.Hotel);
        }

        [HttpPost("partner")]
        //POST : /api/admin/partner
        [Authorize(Roles = "Admin")]
        public async Task<Object> CreatePartner(UserAuthModel model)
        {
            model.Role = "Partner";
            var partnerToBeCreated = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email,
                Partner = new Partner()
                {
                    Hotel = new Hotel()
                    {
                        Name = model.HotelName
                    },
                    partnerStatus = true
                }

            };

            try
            {
                var result = await _userManager.CreateAsync(partnerToBeCreated, model.Password);
                await _userManager.AddToRoleAsync(partnerToBeCreated, model.Role);
                return Ok(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // PUT: api/admin/blockPartner?id=1
        [HttpPut("blockPartner")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BlockPartner(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string partnerId = id;

            var partner = await _context.Partner.FindAsync(partnerId);

            if ( partner == null)
            {
                return NotFound();
            }

            partner.partnerStatus = false;

            _context.Entry(partner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartnerExists(partnerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/admin/unblockPartner?id=1
        [HttpPut("unblockPartner")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnblockPartner(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string partnerId = id;

            var partner = await _context.Partner.FindAsync(partnerId);

            if (partner == null)
            {
                return NotFound();
            }

            partner.partnerStatus = true;

            _context.Entry(partner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartnerExists(partnerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool PartnerExists(string id)
        {
            return _context.Partner.Any(e => e.Id == id);
        }

    }
}