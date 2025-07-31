using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeetingRoom.API.Data;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Controllers
{
    [Route("api/roomfeatures")]
    [ApiController]
    public class RoomFeatureController : ControllerBase
    {
        private readonly SmartMeetingRoomDBContext _context;

        public RoomFeatureController(SmartMeetingRoomDBContext context)
        {
            _context = context;
        }

        // GET: api/roomfeatures
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomFeature>>> GetRoomFeatures()
        {
            return await _context.RoomFeatures.ToListAsync();
        }

        // GET: api/roomfeatures/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomFeature>> GetRoomFeature(int id)
        {
            var roomFeature = await _context.RoomFeatures.FindAsync(id);

            if (roomFeature == null)
            {
                return NotFound();
            }

            return roomFeature;
        }

        // PUT: api/roomfeatures/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomFeature(int id, RoomFeature roomFeature)
        {
            if (id != roomFeature.RoomFeatureId)
            {
                return BadRequest();
            }

            _context.Entry(roomFeature).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomFeatureExists(id))
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

        // POST: api/roomfeatures
        [HttpPost]
        public async Task<ActionResult<RoomFeature>> PostRoomFeature(RoomFeature roomFeature)
        {
            _context.RoomFeatures.Add(roomFeature);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomFeature", new { id = roomFeature.RoomFeatureId }, roomFeature);
        }

        // DELETE: api/roomfeatures/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomFeature(int id)
        {
            var roomFeature = await _context.RoomFeatures.FindAsync(id);
            if (roomFeature == null)
            {
                return NotFound();
            }

            _context.RoomFeatures.Remove(roomFeature);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomFeatureExists(int id)
        {
            return _context.RoomFeatures.Any(e => e.RoomFeatureId == id);
        }
    }
}
