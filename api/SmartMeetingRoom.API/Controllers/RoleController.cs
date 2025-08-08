using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartMeetingRoom.API.Data;
using SmartMeetingRoom.API.DTOs.Role;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly SmartMeetingRoomDBContext _context;
        private readonly IMapper _mapper;

        public RoleController(SmartMeetingRoomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            var roleDtos = _mapper.Map<List<RoleDto>>(roles);
            return Ok(roleDtos);
        }

        // GET: api/roles/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(byte id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            var roleDto = _mapper.Map<RoleDto>(role);
            return Ok(roleDto);
        }

        // PUT: api/roles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(byte id, RoleUpdateDto updatedRoleDto)
        {
            var existingRole = await _context.Roles.FindAsync(id);

            if (existingRole == null)
            {
                return NotFound();
            }

            _mapper.Map(updatedRoleDto, existingRole);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
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

        // POST: api/roles
        [HttpPost]
        public async Task<ActionResult<RoleDto>> PostRole(RoleCreateDto createdRoleDto)
        {
            var role = _mapper.Map<Role>(createdRoleDto);

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var roleDto = _mapper.Map<RoleDto>(role);
            return CreatedAtAction(nameof(GetRole), new { id = role.RoleId }, roleDto);
        }

        // DELETE: api/roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(byte id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(byte id)
        {
            return _context.Roles.Any(e => e.RoleId == id);
        }
    }
}
