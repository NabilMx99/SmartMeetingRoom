using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SmartMeetingRoom.API.Data;
using SmartMeetingRoom.API.DTOs.Role;
using SmartMeetingRoom.API.DTOs.User;

namespace SmartMeetingRoom.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly SmartMeetingRoomDBContext _context;
        private readonly IMapper _mapper;

        public UserController(SmartMeetingRoomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/users
        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.FkRole)
                .Select(u => new UserDto
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber!,
                    Email = u.Email!,
                    IsOnline = u.IsOnline,
                    Role = new RoleDto
                    {
                        RoleId = u.FkRole.Id,
                        RoleName = u.FkRole.Name!,
                        RoleDescription = string.IsNullOrEmpty(u.FkRole.RoleDescription) ? "No description provided." : u.FkRole.RoleDescription
                    }
                }).ToListAsync();

            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.FkRole)
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber!,
                    Email = u.Email!,
                    IsOnline = u.IsOnline,
                    Role = new RoleDto
                    {
                        RoleId = u.FkRole.Id,
                        RoleName = u.FkRole.Name!,
                        RoleDescription = string.IsNullOrEmpty(u.FkRole.RoleDescription) ? "No description provided." : u.FkRole.RoleDescription
                    }
                }).FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && loggedInUserId != id.ToString())
            {
                return Forbid();
            }

            return Ok(user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] UserProfileDto userProfileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && loggedInUserId != id.ToString())
            {
                return Forbid();
            }

            _mapper.Map(userProfileDto, user);

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var loggedInUserIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!User.IsInRole("Admin") && loggedInUserIdStr != id.ToString())
                return Forbid();

            var user = await _context.Users
                .Include(u => u.Attendees)
                .Include(u => u.MeetingMinutes)
                    .ThenInclude(mm => mm.ActionItems)
                .Include(u => u.ActionItems)
                .Include(u => u.RefreshTokens)
                .Include(u => u.Meetings)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            if (user.Meetings.Any())
            {
                return BadRequest("User cannot be deleted while they are organizer of meetings. Reassign or delete those meetings first.");
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (user.Attendees.Any())
                    _context.Attendees.RemoveRange(user.Attendees);

                if (user.ActionItems.Any())
                    _context.ActionItems.RemoveRange(user.ActionItems);

                if (user.MeetingMinutes.Any())
                {
                    var allActionItemsFromMinutes = user.MeetingMinutes.SelectMany(mm => mm.ActionItems).ToList();
                    if (allActionItemsFromMinutes.Any())
                        _context.ActionItems.RemoveRange(allActionItemsFromMinutes);

                    _context.MeetingMinutes.RemoveRange(user.MeetingMinutes);
                }

                if (user.RefreshTokens.Any())
                    _context.RefreshTokens.RemoveRange(user.RefreshTokens);

                _context.Users.Remove(user);
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

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

