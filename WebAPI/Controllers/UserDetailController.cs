using Data.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos;
using WebAPI.Security;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailController : ControllerBase
    {
        private readonly ProjektRwaContext _context;
        private readonly IConfiguration _config;

        public UserDetailController(ProjektRwaContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("[action]")]
        public ActionResult<UserDetailDto> Register(UserDetailDto userDetailDto)
        {
            try
            {
                var trimmedUsername = userDetailDto.Username.Trim();
                if (_context.UserDetails.Any(x => x.Username.Equals(trimmedUsername)))
                {
                    return BadRequest($"{trimmedUsername} already exists");
                }

                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(userDetailDto.Password, b64salt);

                var newUser = new UserDetail
                { 
                    Username = userDetailDto.Username,
                    PasswordHash = b64hash,
                    PasswordSalt = b64salt,
                    Email = userDetailDto.Email,
                    Phone = userDetailDto.Phone,
                    FirstName = userDetailDto.FirstName,
                    LastName = userDetailDto.LastName,
                    UserRoleId = 1
                };

                _context.Add(newUser);
                _context.SaveChanges();

                userDetailDto.DetailId = newUser.UserDetailId;
                return Ok(userDetailDto);

            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);  
            }
        }

        [HttpPost("[action]")]
        public ActionResult GetToken()
        {
            try
            {
                var secureKey = _config["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 10);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult Login(LoginDto loginDto)
        {
            try
            {
                var genericMessage = "Invalid username or password";

                var existingUser = _context.UserDetails
                    .FirstOrDefault(x => x.Username.Equals(loginDto.Username));
                if (existingUser == null)
                {
                    return BadRequest(genericMessage);
                }

                var b64hash = PasswordHashProvider.GetHash(loginDto.Password, existingUser.PasswordSalt);
                if (b64hash != existingUser.PasswordHash)
                {
                    return BadRequest(genericMessage);
                }

                var secureKey = _config["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 10, loginDto.Username);
           
                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult ChangePassword(PasswordDto passwordDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(passwordDto.Username) || string.IsNullOrWhiteSpace(passwordDto.OldPassword) || string.IsNullOrWhiteSpace(passwordDto.NewPassword))
                {
                    return BadRequest("There should be input");
                }

                var existingUser = _context.UserDetails.
                    FirstOrDefault(x => x.Username == passwordDto.Username);
                if (existingUser == null)
                {
                    return BadRequest("User does not exist");
                }

                var currentHash = PasswordHashProvider.GetHash(passwordDto.OldPassword, existingUser.PasswordSalt);
                if (currentHash != existingUser.PasswordHash)
                {
                    return BadRequest("Invalid password");
                }

                var newSalt = PasswordHashProvider.GetSalt();
                var newHash = PasswordHashProvider.GetHash(passwordDto.NewPassword, newSalt);

                existingUser.PasswordSalt = newSalt;
                existingUser.PasswordHash = newHash;

                _context.Update(existingUser);
                _context.SaveChanges();

                return Ok("Password changed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
