using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeetingRoom.API.Data;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Controllers
{
    [Route("api/meetingminutes")]
    [ApiController]
    public class MeetingMinutesController : ControllerBase
    {
        private readonly SmartMeetingRoomDBContext _context;

        public MeetingMinutesController(SmartMeetingRoomDBContext context)
        {
            _context = context;
        }

        // GET: api/meetingminutes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeetingMinute>>> GetMeetingMinutes()
        {
            return await _context.MeetingMinutes.ToListAsync();
        }

        // GET: api/meetingminutes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MeetingMinute>> GetMeetingMinute(int id)
        {
            var meetingMinute = await _context.MeetingMinutes.FindAsync(id);

            if (meetingMinute == null)
            {
                return NotFound();
            }

            return meetingMinute;
        }

        // PUT: api/meetingminutes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeetingMinute(int id, MeetingMinute meetingMinute)
        {
            if (id != meetingMinute.MeetingMinutesId)
            {
                return BadRequest();
            }

            _context.Entry(meetingMinute).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeetingMinuteExists(id))
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

        // POST: api/meetingminutes
        [HttpPost]
        public async Task<ActionResult<MeetingMinute>> PostMeetingMinute(MeetingMinute meetingMinute)
        {
            _context.MeetingMinutes.Add(meetingMinute);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeetingMinute", new { id = meetingMinute.MeetingMinutesId }, meetingMinute);
        }

        // DELETE: api/meetingminutes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeetingMinute(int id)
        {
            var meetingMinute = await _context.MeetingMinutes.FindAsync(id);
            if (meetingMinute == null)
            {
                return NotFound();
            }

            _context.MeetingMinutes.Remove(meetingMinute);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MeetingMinuteExists(int id)
        {
            return _context.MeetingMinutes.Any(e => e.MeetingMinutesId == id);
        }
    }
}
