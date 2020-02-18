using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ddacAPI.Data;
using ddacAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ddacAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ddacAPIContext _context;

        public PartnersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ddacAPIContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/Partners
        [HttpGet]
        public IEnumerable<Partner> GetPartner()
        {
            return _context.Partner;
        }

        // GET: api/Partners/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPartner([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var partner = await _context.Partner.FindAsync(id);

            if (partner == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            var hotel = await _context.Hotel.FindAsync(partner.HotelId);
            //var hotel = _context.Hotel.Include(i => i.Messages).FirstOrDefaultAsync(i => i.Id == id);

            return Ok( new
            {
                user.Email,
                user.UserName,
                hotel
                
            });

        }

        // PUT: api/Partners/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPartner([FromRoute] string id, [FromBody] Partner partner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != partner.Id)
            {
                return BadRequest();
            }

            _context.Entry(partner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartnerExists(id))
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


        // DELETE: api/Partners/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartner([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var partner = await _context.Partner.FindAsync(id);
            if (partner == null)
            {
                return NotFound();
            }

            _context.Partner.Remove(partner);
            await _context.SaveChangesAsync();

            return Ok(partner);
        }

        private bool PartnerExists(string id)
        {
            return _context.Partner.Any(e => e.Id == id);
        }

        [HttpPost]
        //POST : /api/Customer/signup
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
                    }
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
    }
}