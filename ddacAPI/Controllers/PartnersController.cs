using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ddacAPI.Data;
using ddacAPI.Models;

namespace ddacAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private readonly ddacAPIContext _context;

        public PartnersController(ddacAPIContext context)
        {
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
        public async Task<IActionResult> GetPartner([FromRoute] int id)
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

            return Ok(partner);
        }

        // PUT: api/Partners/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPartner([FromRoute] int id, [FromBody] Partner partner)
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

        // POST: api/Partners
        [HttpPost]
        public async Task<IActionResult> PostPartner([FromBody] Partner partner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Partner.Add(partner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPartner", new { id = partner.Id }, partner);
        }

        // DELETE: api/Partners/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartner([FromRoute] int id)
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

        private bool PartnerExists(int id)
        {
            return _context.Partner.Any(e => e.Id == id);
        }
    }
}