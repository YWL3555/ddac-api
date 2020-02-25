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
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ddacAPI.Util;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ddacAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ddacAPIContext _context;
        private static BlobManager _blobManager;

        public IConfiguration Configuration { get; }

        public PartnerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ddacAPIContext context, IConfiguration configuration)
        {
            Configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        //POST: /api/partner/login
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

        // GET: api/partner/roomType
        [HttpGet]
        [Authorize(Roles = "Partner")]
        [Route("roomType")]
        //GET: /api/partner/room-type
        public async Task<Object> GetRoomTypes()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var partner = await _context.Partner.FindAsync(userId);

            var roomTypes = _context.RoomType.Where(r => r.HotelId == partner.HotelId);

            return roomTypes;
        }

        // POST: api/partner/roomType
        [HttpPost]
        [Authorize(Roles = "Partner")]
        [Route("roomType")]
        public async Task<IActionResult> PostRoomType([FromBody] RoomType roomType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var partner = await _context.Partner.FindAsync(userId);

            var roomTypeToBeCreated = new RoomType()
            {
                HotelId = partner.HotelId,
                Name = roomType.Name,
                Price = roomType.Price,
                Quantity = roomType.Quantity,
                Photo = roomType.Photo,
                MaximumPax = roomType.MaximumPax
            };

            _context.RoomType.Add(roomTypeToBeCreated);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomType", new { id = roomType.Id }, roomType);
        }

        // PUT: api/partner/RoomType/5
        [HttpPut("roomType/{id}")]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> PutRoomType([FromRoute] int id, [FromBody] RoomType roomType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != roomType.Id)
            {
                return BadRequest();
            }
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var partner = await _context.Partner.FindAsync(userId);

            if (roomType.HotelId != partner.HotelId)
            {
                return Forbid();
            }

            _context.Entry(roomType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomTypeExists(id))
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

        // DELETE: api/partner/RoomType/5
        [HttpDelete("roomType/{id}")]
        public async Task<IActionResult> DeleteRoomType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roomType = await _context.RoomType.FindAsync(id);
            if (roomType == null)
            {
                return NotFound();
            }

            _context.RoomType.Remove(roomType);
            await _context.SaveChangesAsync();

            return Ok(roomType);
        }

        // GET: api/partner/RatingReview
        [HttpGet("ratingReview")]
        public object GetRatingReviewByHotel()
        {
            string partnerId = User.Claims.First(c => c.Type == "UserID").Value;

            var partner = _context.Partner.FindAsync(partnerId);

            int currentHotelId = partner.Result.HotelId;

            var ratingReviews = _context.RatingReview.Include(r => r.Customer).Where(r => r.HotelId == currentHotelId);

            if (ratingReviews == null)
            {
                return NotFound();
            }

            return ratingReviews;
        }

        // GET: api/partner/Booking
        [HttpGet("Booking")]
        public object GetBookingsByHotel()
        {

            string partnerId = User.Claims.First(c => c.Type == "UserID").Value;

            var partner = _context.Partner.FindAsync(partnerId);

            int currentHotelId = partner.Result.HotelId;

            var bookings = _context.Booking.Include(b => b.Customer).Include(b => b.RoomType).Where(b => b.RoomType.HotelId == currentHotelId);

            if (bookings == null)
            {
                return NotFound();
            }

            return bookings;
        }

        [HttpGet]
        [Authorize(Roles = "Partner")]
        [Route("profile")]
        //GET: /api/partner/profile
        public async Task<Object> GetHotelProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var partner = _context.Partner.FindAsync(userId);
            var hotel = await _context.Hotel.FindAsync(partner.Result.HotelId);

            return hotel;
        }

        [HttpPut]
        [Authorize(Roles = "Partner")]
        [Route("profile")]
        //PUT: /api/partner/profile
        public async Task<IActionResult> EditHotelProfile([FromBody] Hotel hotel)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(hotel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpPost]
        [Authorize(Roles = "Partner")]
        [Route("pic-upload")]
        //GET: /api/partner/pic-upload
        public async Task<IActionResult> UploadUserProfilePic(IFormFile newFile)
        {
            _blobManager = new BlobManager(Configuration);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Claims.First(c => c.Type == "UserID").Value;

            var partner = await _context.Partner.FindAsync(userId);

            var hotel = await _context.Hotel.FindAsync(partner.HotelId);


            if (newFile != null)
            {
                Stream filestreamFromRequest = newFile.OpenReadStream();
                hotel.Photo = await _blobManager.UploadFileToStorageAsync(filestreamFromRequest, "hotel_" + hotel.Id.ToString() + ".jpg");
            }
            else
            {
                return BadRequest();
            }


            _context.Entry(hotel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();

        }

        // PUT: api/partner/publish
        [HttpPut("publish")]
        public async Task<IActionResult> PublishHotel()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var partner = _context.Partner.FindAsync(userId);
            var hotel = await _context.Hotel.FindAsync(partner.Result.HotelId);

            if (hotel == null)
            {
                return NotFound();
            }

            hotel.Published = true;

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(hotel.Id))
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

        // PUT: api/partner/unpublish
        [HttpPut("unpublish")]
        public async Task<IActionResult> UnpublishHotel()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var partner = _context.Partner.FindAsync(userId);
            var hotel = await _context.Hotel.FindAsync(partner.Result.HotelId);

            if (hotel == null)
            {
                return NotFound();
            }

            hotel.Published = true;

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(hotel.Id))
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

        private bool RoomTypeExists(int id)
        {
            return _context.RoomType.Any(e => e.Id == id);
        }

        private bool HotelExists(int id)
        {
            return _context.Hotel.Any(e => e.Id == id);
        }
    }
}