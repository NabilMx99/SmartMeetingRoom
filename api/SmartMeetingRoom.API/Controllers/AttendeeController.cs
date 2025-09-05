using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SmartMeetingRoom.API.Data;
using SmartMeetingRoom.API.DTOs.Attendee;

namespace SmartMeetingRoom.API.Controllers
{
    [Route("api/attendees")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class AttendeeController : ControllerBase
    {
        private readonly SmartMeetingRoomDBContext _context;
        private readonly IMapper _mapper;

        public AttendeeController(SmartMeetingRoomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/attendees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendeeDto>>> GetAttendees()
        {
            var attendees = await _context.Attendees
                .Include(a => a.FkUser)
                    .ThenInclude(u => u.FkRole)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<AttendeeDto>>(attendees));
        }

        // GET: api/attendees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AttendeeDto>> GetAttendee(int id)
        {
            var attendee = await _context.Attendees
                .Include(a => a.FkUser)
                    .ThenInclude(u => u.FkRole)
                .FirstOrDefaultAsync(a => a.AttendeeId == id);

            if (attendee == null) return NotFound();

            return Ok(_mapper.Map<AttendeeDto>(attendee));
        }

        // PUT: api/attendees/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendeeStatus(int id, [FromBody] AttendeeUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var attendee = await _context.Attendees
                .Include(a => a.FkUser)
                .FirstOrDefaultAsync(a => a.AttendeeId == id);

            if (attendee == null)
                return NotFound();

            _mapper.Map(updateDto, attendee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Attendees.AnyAsync(a => a.AttendeeId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }
    }
}
