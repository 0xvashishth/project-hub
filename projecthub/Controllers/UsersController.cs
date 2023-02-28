using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.PointsToAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using projecthub.Models;

namespace projecthub.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ProjectContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(ProjectContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration  = configuration;
        }

        // GET: api/Users
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.Select(x => UserToDto(x)).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> GetUser(long id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return UserToDto(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, UserUpdateDTO user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            var userObj = await _context.Users.FindAsync(id);

            if (userObj == null)
            {
                return NotFound();
            }
            userObj.Name = user.Name;
            userObj.Github = user.Github;
            userObj.Linkedin = user.Linkedin;
            userObj.Twitter = user.Twitter;

            _context.Entry(userObj).State = EntityState.Modified;

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

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // register user
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> Register(UserDTO user, string password)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ProjectContext.Users'  is null.");
            }
            var userExi = await UserExist(user.Email);
            if (userExi)
            {
                return BadRequest($"Email already exist!!");
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            var userToAdd = new User(user.Id, user.Email, user.Name, user.Twitter, user.Linkedin, user.Github, passwordHash, passwordSalt);
            var getSecretToken = CreateToken(userToAdd);
            userToAdd.Secret_token = getSecretToken;
            _context.Users.Add(userToAdd);
            await _context.SaveChangesAsync();

            UserLoginSendDTO userobj = new UserLoginSendDTO();
            userobj.userDTO = UserToDto(userToAdd);
            userobj.token = getSecretToken;
            return Ok(userobj);
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO userDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDTO.Email);
            if (user == null)
            {
                return NotFound();
            }
            else if(!VerifyPasswordHash(userDTO.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest();
            }
            else
            {
                UserLoginSendDTO userobj = new UserLoginSendDTO();
                userobj.userDTO = UserToDto(user);
                userobj.token = CreateToken(user);
                return Ok(userobj);
            }
        }

        private static UserDTO UserToDto(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Twitter = user.Twitter,
                Linkedin = user.Linkedin,
                Points = user.Points,
                Projects = user.Projects,
                Github = user.Github
            };
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value!;
            if (appSettingsToken == null)
            {
                throw new Exception("AppSetting token is null");
            }
            SymmetricSecurityKey symmetricsecuritykey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingsToken));
            SigningCredentials signingCredentials = new SigningCredentials(symmetricsecuritykey, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials= signingCredentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(salt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(hash);
            }
        }

        private async Task<bool> UserExist(string email)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            return false;
        }

        private bool UserExists(long id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
