using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using SmartMeetingRoom.API.Data;
using SmartMeetingRoom.API.DTOs.User;
using SmartMeetingRoom.API.DTOs.User.Auth;
using SmartMeetingRoom.API.Models;

namespace SmartMeetingRoom.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SmartMeetingRoomDBContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SmartMeetingRoomDBContext context,
            IMapper mapper,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserSignUpDto signUpDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(signUpDto.Email);
            if (existingUser != null)
                return BadRequest("Email is already in use.");

            string defaultRoleName = "Guest";

            var role = await _roleManager.FindByNameAsync(defaultRoleName);
            if (role == null)
            {
                var newRole = new ApplicationRole
                {
                    Name = defaultRoleName
                };
                var roleResult = await _roleManager.CreateAsync(newRole);
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                        ModelState.AddModelError(error.Code, error.Description);
                    return BadRequest(ModelState);
                }
                role = newRole;
            }

            var user = new ApplicationUser
            {
                FkRoleId = role.Id,
                UserName = signUpDto.Email,
                Email = signUpDto.Email,
                FirstName = signUpDto.FirstName,
                LastName = signUpDto.LastName,
                PhoneNumber = signUpDto.PhoneNumber,
                IsOnline = false,
            };

            var createResult = await _userManager.CreateAsync(user, signUpDto.Password);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);

                return BadRequest(ModelState);
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, defaultRoleName);
            if (!addToRoleResult.Succeeded)
            {
                foreach (var error in addToRoleResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return BadRequest(ModelState);
            }

            user.FkRoleId = role.Id;
            await _userManager.UpdateAsync(user);

            return Ok(new { Message = "User registered successfully." });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
                return Unauthorized("Invalid email or password.");

            var roles = await _userManager.GetRolesAsync(user);

            var token = GenerateJwtToken(user, roles);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Role = new DTOs.Role.RoleDto
            {
                RoleName = roles.Count > 0 ? roles[0] : "User"
            };

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var refreshToken = GenerateRefreshToken(ipAddress);
            await _context.Entry(user).Collection(u => u.RefreshTokens).LoadAsync();
            user.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                User = userDto
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.Token);

            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.Expires <= DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token.");

            var user = refreshToken.User;

            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            var roles = await _userManager.GetRolesAsync(user);
            var newJwtToken = GenerateJwtToken(user, roles);
            var newRefreshToken = GenerateRefreshToken(request.IpAddress);

            user.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return Ok(new RefreshTokenResponseDto
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequestDto request)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == request.Token);

            if (refreshToken == null || refreshToken.IsRevoked)
                return NotFound("Refresh token not found or already revoked.");

            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Refresh token revoked successfully." });
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var randomBytes = new byte[64];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                IsRevoked = false
            };
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims.Concat(roleClaims),
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}