using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SmartMeetingRoom.API.Data;
using SmartMeetingRoom.API.DTOs.Role;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Controllers
{
    [Route("api/roles")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly SmartMeetingRoomDBContext _context;
        private readonly IMapper _mapper;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleController(
            SmartMeetingRoomDBContext context,
            IMapper mapper,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();

            var roleDtos = roles.Select(r => new RoleDto
            {
                RoleId = r.Id,
                RoleName = r.Name!,
                RoleDescription = string.IsNullOrEmpty(r.RoleDescription) ? "No description provided." : r.RoleDescription
            }).ToList();

            return Ok(roleDtos);
        }

        // GET: api/roles/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound();

            var roleDto = new RoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name!,
                RoleDescription = string.IsNullOrEmpty(role.RoleDescription) ? "No description provided." : role.RoleDescription
            };

            return Ok(roleDto);
        }

        // PUT: api/roles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, [FromBody] RoleUpdateDto updatedRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingRole = await _context.Roles.FindAsync(id);
            if (existingRole == null)
                return NotFound();

            _mapper.Map(updatedRoleDto, existingRole);

            existingRole.NormalizedName = _roleManager.NormalizeKey(existingRole.Name!);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // POST: api/roles
        [HttpPost]
        public async Task<ActionResult<RoleDto>> PostRole([FromBody] RoleCreateDto createdRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = _mapper.Map<ApplicationRole>(createdRoleDto);

            role.NormalizedName = _roleManager.NormalizeKey(role.Name!);

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var roleDto = new RoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name!,
                RoleDescription = string.IsNullOrEmpty(role.RoleDescription) ? "No description provided." : role.RoleDescription
            };

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, roleDto);
        }

        // DELETE: api/roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles
               .Include(r => r.Users)
               .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return NotFound();

            if (role.Users.Any())
                return BadRequest("Cannot delete role while users are assigned to it. Reassign or remove users first.");

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
