using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using SmartMeetingRoom.API.Data;
using SmartMeetingRoom.API.DTOs.Meeting;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Controllers
{
    [Route("api/meetings")]
    [ApiController]
    [Authorize]
    public class MeetingController : ControllerBase
    {
        private readonly SmartMeetingRoomDBContext _context;
        private readonly IMapper _mapper;

        public MeetingController(SmartMeetingRoomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/meetings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeetingDto>>> GetMeetings()
        {
            var meetings = await _context.Meetings
               .Include(m => m.FkUser).ThenInclude(u => u.FkRole)
               .Include(m => m.FkRoom).ThenInclude(r => r.RoomFeatures).ThenInclude(rf => rf.FkFeature)
               .Include(m => m.Attendees).ThenInclude(a => a.FkUser).ThenInclude(u => u.FkRole)
               .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<MeetingDto>>(meetings));
        }

        // GET: api/meetings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MeetingDto>> GetMeeting(int id)
        {
            var meeting = await _context.Meetings
                .Include(m => m.FkUser).ThenInclude(u => u.FkRole)
                .Include(m => m.FkRoom).ThenInclude(r => r.RoomFeatures).ThenInclude(rf => rf.FkFeature)
                .Include(m => m.Attendees).ThenInclude(a => a.FkUser).ThenInclude(u => u.FkRole)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            if (meeting == null) return NotFound();

            return Ok(_mapper.Map<MeetingDto>(meeting));
        }

        // POST: api/meetings
        [HttpPost]
        public async Task<ActionResult<MeetingDto>> PostMeeting([FromBody] MeetingCreateDto meetingCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var organizer = await _context.Users
                .Include(u => u.FkRole)
                .FirstOrDefaultAsync(u => u.Id == loggedInUserId);

            if (organizer == null)
                return Unauthorized("Logged-in user not found.");

            bool roomIsBooked = await _context.Meetings.AnyAsync(m =>
                m.FkRoomId == meetingCreateDto.FkRoomId &&
                m.MeetingStatus != "Cancelled" &&
                (meetingCreateDto.MeetingStartTime < m.MeetingEndTime) &&
                (meetingCreateDto.MeetingEndTime > m.MeetingStartTime)
            );

            if (roomIsBooked)
                return BadRequest("The selected room is already booked for the given time slot.");

            var meeting = _mapper.Map<Meeting>(meetingCreateDto);
            meeting.FkUserId = organizer.Id;
            meeting.FkUser = organizer;

            var room = await _context.Rooms
                .Include(r => r.RoomFeatures)
                    .ThenInclude(rf => rf.FkFeature)
                .FirstOrDefaultAsync(r => r.RoomId == meetingCreateDto.FkRoomId);

            if (room == null)
                return BadRequest("The selected room does not exist.");

            meeting.FkRoom = room;

            if (meetingCreateDto.AttendeeUserIds != null && meetingCreateDto.AttendeeUserIds.Any())
            {
                var attendees = await _context.Users
                    .Where(u => meetingCreateDto.AttendeeUserIds.Contains(u.Id))
                    .Include(u => u.FkRole)
                    .ToListAsync();

                foreach (var user in attendees)
                {
                    meeting.Attendees.Add(new Attendee
                    {
                        FkUserId = user.Id,
                        FkUser = user,
                        AttendeeStatus = "Invited"
                    });
                }
            }

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            var meetingDto = await _context.Meetings
                .Include(m => m.FkUser).ThenInclude(u => u.FkRole)
                .Include(m => m.FkRoom).ThenInclude(r => r.RoomFeatures).ThenInclude(rf => rf.FkFeature)
                .Include(m => m.Attendees).ThenInclude(a => a.FkUser).ThenInclude(u => u.FkRole)
                .FirstOrDefaultAsync(m => m.MeetingId == meeting.MeetingId);

            return CreatedAtAction(nameof(GetMeeting), new { id = meeting.MeetingId }, _mapper.Map<MeetingDto>(meetingDto));
        }

        // PUT: api/meetings/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeeting(int id, [FromBody] MeetingUpdateDto meetingUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var meeting = await _context.Meetings
                .Include(m => m.Attendees)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            if (meeting == null) return NotFound();

            var effectiveStart = meetingUpdateDto.MeetingStartTime ?? meeting.MeetingStartTime;
            var effectiveEnd = meetingUpdateDto.MeetingEndTime ?? meeting.MeetingEndTime;
            var roomId = meeting.FkRoomId;

            bool roomIsBooked = await _context.Meetings.AnyAsync(m =>
                m.FkRoomId == roomId &&
                m.MeetingId != id &&
                m.MeetingStatus != "Cancelled" &&
                effectiveStart < m.MeetingEndTime &&
                effectiveEnd > m.MeetingStartTime
            );

            if (roomIsBooked)
                return BadRequest("The selected room is already booked for the given time slot.");

            _mapper.Map(meetingUpdateDto, meeting);

            if (meetingUpdateDto.AttendeeUserIds != null)
            {
                meeting.Attendees.Clear();

                var attendees = await _context.Users
                    .Where(u => meetingUpdateDto.AttendeeUserIds.Contains(u.Id))
                    .Include(u => u.FkRole)
                    .ToListAsync();

                foreach (var user in attendees)
                {
                    meeting.Attendees.Add(new Attendee
                    {
                        FkUserId = user.Id,
                        FkUser = user,
                        AttendeeStatus = "Invited"
                    });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Meetings.Any(m => m.MeetingId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/meetings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeeting(int id)
        {
            var meeting = await _context.Meetings
                .Include(m => m.Attendees)
                .Include(m => m.MeetingMinutes)
                    .ThenInclude(mm => mm.ActionItems)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            if (meeting == null) return NotFound();

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                if (meeting.MeetingMinutes != null && meeting.MeetingMinutes.Any())
                {
                    var actionItems = meeting.MeetingMinutes.SelectMany(mm => mm.ActionItems).ToList();
                    if (actionItems.Any())
                        _context.ActionItems.RemoveRange(actionItems);

                    _context.MeetingMinutes.RemoveRange(meeting.MeetingMinutes);
                }

                if (meeting.Attendees.Any())
                    _context.Attendees.RemoveRange(meeting.Attendees);

                _context.Meetings.Remove(meeting);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
                return NoContent();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
