using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SmartMeetingRoom.API.Data;
using SmartMeetingRoom.API.DTOs.Room;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly SmartMeetingRoomDBContext _context;
        private readonly IMapper _mapper;

        public RoomController(SmartMeetingRoomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/rooms
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomFeatures)
                .ThenInclude(rf => rf.FkFeature)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<RoomDto>>(rooms));
        }

        // GET: api/rooms/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<RoomDto>> GetRoom(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomFeatures)
                    .ThenInclude(rf => rf.FkFeature)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound();

            return Ok(_mapper.Map<RoomDto>(room));
        }

        // PUT: api/rooms/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> PutRoom(int id, [FromBody] RoomUpdateDto roomUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = await _context.Rooms
                .Include(r => r.RoomFeatures)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null) return NotFound();

            if (roomUpdateDto.RoomName != null)
                room.RoomName = roomUpdateDto.RoomName;

            if (roomUpdateDto.RoomLocation != null)
                room.RoomLocation = roomUpdateDto.RoomLocation;

            if (roomUpdateDto.RoomCapacity.HasValue)
                room.RoomCapacity = roomUpdateDto.RoomCapacity.Value;

            if (roomUpdateDto.IsAvailable.HasValue)
                room.IsAvailable = roomUpdateDto.IsAvailable.Value;

            if (roomUpdateDto.FeatureIds != null)
            {
                _context.RoomFeatures.RemoveRange(room.RoomFeatures);

                room.RoomFeatures = roomUpdateDto.FeatureIds
                    .Select(fid => new RoomFeature
                    {
                        FkRoomId = id,
                        FkFeatureId = fid
                    })
                    .ToList();
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/rooms
        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<RoomDto>> PostRoom([FromBody] RoomCreateDto roomCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = _mapper.Map<Room>(roomCreateDto);

            foreach (var featureId in roomCreateDto.FeatureIds)
            {
                room.RoomFeatures.Add(new RoomFeature { FkFeatureId = featureId });
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var roomWithFeatures = await _context.Rooms
                .Include(r => r.RoomFeatures)
                    .ThenInclude(rf => rf.FkFeature)
                .FirstOrDefaultAsync(r => r.RoomId == room.RoomId);

            var roomDto = _mapper.Map<RoomDto>(roomWithFeatures);
            return CreatedAtAction(nameof(GetRoom), new { id = room.RoomId }, roomDto);
        }

        // DELETE: api/rooms/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomFeatures)
                .Include(r => r.Meetings)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null) return NotFound();

            if (room.Meetings.Any())
            {
                return BadRequest("Cannot delete room while there are meetings scheduled. Remove or reassign those meetings first.");
            }

            if (room.RoomFeatures.Any())
            {
                _context.RoomFeatures.RemoveRange(room.RoomFeatures);
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }
    }
}