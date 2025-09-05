using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SmartMeetingRoom.API.Data;
using SmartMeetingRoom.API.DTOs.Feature;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Controllers
{
    [Route("api/features")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        private readonly SmartMeetingRoomDBContext _context;
        private readonly IMapper _mapper;

        public FeatureController(SmartMeetingRoomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/features
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FeatureDto>>> GetFeatures()
        {
            var features = await _context.Features.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<FeatureDto>>(features));
        }

        // GET: api/features/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<FeatureDto>> GetFeature(byte id)
        {
            var feature = await _context.Features.FindAsync(id);

            if (feature == null)
                return NotFound();

            return Ok(_mapper.Map<FeatureDto>(feature));
        }

        // PUT: api/features/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> PutFeature(byte id, [FromBody] FeatureUpdateDto featureUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var feature = await _context.Features.FindAsync(id);
            if (feature == null) return NotFound();

            _mapper.Map(featureUpdateDto, feature);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeatureExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/features
        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<FeatureDto>> PostFeature([FromBody] FeatureCreateDto featureCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var feature = _mapper.Map<Feature>(featureCreateDto);

            _context.Features.Add(feature);
            await _context.SaveChangesAsync();

            var featureDto = _mapper.Map<FeatureDto>(feature);
            return CreatedAtAction(nameof(GetFeature), new { id = feature.FeatureId }, featureDto);
        }

        // DELETE: api/features/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteFeature(byte id)
        {
            var isUsed = await _context.RoomFeatures.AnyAsync(rf => rf.FkFeatureId == id);
            if (isUsed)
                return BadRequest("Feature cannot be deleted because it is assigned to one or more rooms. Remove those associations first.");

            var feature = await _context.Features.FindAsync(id);
            if (feature == null) return NotFound();

            _context.Features.Remove(feature);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeatureExists(byte id)
        {
            return _context.Features.Any(e => e.FeatureId == id);
        }
    }
}
