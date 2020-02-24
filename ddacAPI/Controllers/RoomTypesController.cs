using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ddacAPI.Data;
using ddacAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ddacAPI.Util;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ddacAPI.Controllers
{
    //[Authorize(Roles = "Partner")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypesController : ControllerBase
    {
        private readonly ddacAPIContext _context;
        private static BlobManager _blobManager;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        public IConfiguration Configuration { get; }

        public RoomTypesController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ddacAPIContext context, IConfiguration configuration)
        {
            Configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/RoomTypes
        [HttpGet]
        public IEnumerable<RoomType> GetRoomType()
        {
            return _context.RoomType;
        }

        // GET: api/RoomTypes/ByHotel
        [HttpGet("ByHotel")]
        public IEnumerable<RoomType> GetRoomTypeByHotel(int id)
        {

            var roomTypes = _context.RoomType.Where(r => r.HotelId == id);

            return roomTypes;
        }

        // GET: api/RoomTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomType([FromRoute] int id)
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

            return Ok(roomType);
        }

        // PUT: api/RoomTypes/5
        [HttpPut("{id}")]
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

        // POST: api/RoomTypes
        [HttpPost]
        [Authorize(Roles = "Partner")]
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

        // DELETE: api/RoomTypes/5
        [HttpDelete("{id}")]
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

        [HttpPost]
        [Authorize(Roles = "Partner")]
        [Route("upload")]
        //GET: /api/roomTypes/upload?id=1
        public async Task<IActionResult> UploadUserProfilePic(int id, IFormFile newFile)
        {
            _blobManager = new BlobManager(Configuration);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int currentRoomTypeId = id;

            var currentRoomType = await _context.RoomType.FindAsync(currentRoomTypeId);


            if (newFile != null)
            {
                Stream filestreamFromRequest = newFile.OpenReadStream();
                currentRoomType.Photo = await _blobManager.UploadFileToStorageAsync(filestreamFromRequest, "room_type_"+currentRoomType.Id.ToString() + ".jpg");
            }
            else
            {
                return BadRequest();
            }


            _context.Entry(currentRoomType).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();

        }

        private bool RoomTypeExists(int id)
        {
            return _context.RoomType.Any(e => e.Id == id);
        }
    }
}