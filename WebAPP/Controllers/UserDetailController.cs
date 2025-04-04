using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPP.Models;
using WebAPP.Security;

namespace WebAPP.Controllers
{
    public class UserDetailController : Controller
    {
        private readonly ProjektRwaContext _context;

        public UserDetailController(ProjektRwaContext context)
        {
            _context = context;
        }

        // GET: UserDetailController
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string url)
        {
            ViewData["HideNavbars"] = true;
            var userLoginVM = new UserLoginVM
            { 
                Url = url
            };
            return View(userLoginVM);
        }

        [HttpPost]
        public IActionResult Login(UserLoginVM userLoginVM)
        {
            var existingUser = _context.UserDetails.Include(x => x.UserRole)
                .FirstOrDefault(x => x.Username == userLoginVM.Username);
            if (existingUser == null)
            { 
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var b64hash = PasswordHashProvider.GetHash(userLoginVM.Password, existingUser.PasswordSalt);
            if (b64hash != existingUser.PasswordHash)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var claims = new List<Claim>()
            { 
                new Claim(ClaimTypes.Name, existingUser.Username),
                new Claim(ClaimTypes.Role, existingUser.UserRole.UserRoleName)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            Task.Run(async () =>
            await HttpContext.SignInAsync(
                               CookieAuthenticationDefaults.AuthenticationScheme,
                                              new ClaimsPrincipal(claimsIdentity),
                                                             authProperties)).GetAwaiter().GetResult();

            if (!string.IsNullOrEmpty(userLoginVM.Url))
            {
                return LocalRedirect(userLoginVM.Url);                
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            Task.Run(async () =>
                       await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)).GetAwaiter().GetResult();

            return View();
        }

        public IActionResult Register()
        {
            ViewData["HideNavbars"] = true;
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult Register(UserDetailVM userDetailVM)
        { 
            var trimmedUsername = userDetailVM.Username.Trim();
            if (_context.UserDetails.Any(x => x.Username.Equals(trimmedUsername)))
            {
                ModelState.AddModelError("", "This user already exists");
                return View();
            }

            return RedirectToAction("ConfirmRegistration", userDetailVM);
        }

        public ActionResult ConfirmRegistration(UserDetailVM userDetailVM)
        {
            return View(userDetailVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CompleteRegistration(UserDetailVM userDetailVM)
        {
            var b64salt = PasswordHashProvider.GetSalt();
            var b64hash = PasswordHashProvider.GetHash(userDetailVM.Password, b64salt);
            var userDetail = new UserDetail
            {
                Username = userDetailVM.Username,
                PasswordHash = b64hash,
                PasswordSalt = b64salt,
                FirstName = userDetailVM.FirstName,
                LastName = userDetailVM.LastName,
                Email = userDetailVM.Email,
                Phone = userDetailVM.Phone,
                UserRoleId = 1
            };

            _context.Add(userDetail);
            _context.SaveChanges();
            TempData["SuccesMessage"] = "Registration successful. You can now login.";

            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult ProfileDetails()
        {
            var username = HttpContext.User.Identity.Name;
            var userDetail = _context.UserDetails.FirstOrDefault(x => x.Username == username);
            var userDetailVM = new UserDetailVM
            {
                UserDetailId = userDetail.UserDetailId,
                Username = userDetail.Username,
                Email = userDetail.Email,
                Phone = userDetail.Phone,
                FirstName = userDetail.FirstName,
                LastName = userDetail.LastName
            };

            return View(userDetailVM);
        }

        [Authorize]
        public IActionResult EditProfile(int id)
        {
            var userDetail = _context.UserDetails.First(x => x.UserDetailId == id);
            var userDetailVM = new UserDetailVM
            {
                UserDetailId = userDetail.UserDetailId,
                Username = userDetail.Username,
                Email = userDetail.Email,
                Phone = userDetail.Phone,
                FirstName = userDetail.FirstName,
                LastName = userDetail.LastName
            };

            return View(userDetailVM);
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditProfile(int id, UserDetailVM userVm)
        {
            var userDetail = _context.UserDetails.First(x => x.UserDetailId == id);
            userDetail.Email = userVm.Email;
            userDetail.Phone = userVm.Phone;
            userDetail.FirstName = userVm.FirstName;
            userDetail.LastName = userVm.LastName;

            _context.SaveChanges();
            return RedirectToAction("ProfileDetails");
        }

        public JsonResult GetProfileData(int id)
        { 
            var userDb = _context.UserDetails.First(x => x.UserDetailId == id);
            return Json(new
            {
                userDb.Email,
                userDb.Phone,
                userDb.FirstName,
                userDb.LastName
            });
        }

        [HttpPut]
        public ActionResult SetProfileData(int id, [FromBody] UserDetailVM userVM)
        { 
            var userDb = _context.UserDetails.First(x => x.UserDetailId == id);
            userDb.Email = userVM.Email;
            userDb.Phone = userVM.Phone;
            userDb.FirstName = userVM.FirstName;
            userDb.LastName = userVM.LastName;

            _context.SaveChanges();
            return Ok();
        }

        public IActionResult ProfileDetailsPartial()
        { 
            var username = HttpContext.User.Identity.Name;
            var userDetail = _context.UserDetails.First(x => x.Username == username);
            var userDetailVM = new UserDetailVM
            {
                UserDetailId = userDetail.UserDetailId,
                Username = userDetail.Username,
                Email = userDetail.Email,
                Phone = userDetail.Phone,
                FirstName = userDetail.FirstName,
                LastName = userDetail.LastName
            };

            return PartialView("_ProfileDetailsPartial", userDetailVM);
        }
    }
}
