using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPP.Models;

namespace WebAPP.Controllers
{
    [Authorize]
    public class UserMessageController : Controller
    {
        private readonly ProjektRwaContext _context;

        public UserMessageController(ProjektRwaContext context)
        {
            _context = context;
        }

        // GET: UserMessageController
        public ActionResult Index()
        {
            try
            {
                UserMessageVM newUserMessage = null;

                var userMessageVms = _context.UserMessages.Select(x => new UserMessageVM
                {
                    UserMessageId = x.UserMessageId,
                    Message = x.Message,
                    Username = x.Username
                }).ToList();

                return View(userMessageVms);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: UserMessageController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserMessageController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserMessageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserMessageVM userMessage)
        {
            try
            {
                var newUserMessage = new UserMessage
                {
                    Message = userMessage.Message,
                    Username = userMessage.Username
                };

                _context.UserMessages.Add(newUserMessage);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserMessageController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserMessageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserMessageController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var userMessage = _context.UserMessages.FirstOrDefault(x => x.UserMessageId == id);
                var userMessageVM = new UserMessageVM
                {
                    UserMessageId = userMessage.UserMessageId,
                    Message = userMessage.Message,
                    Username = userMessage.Username
                };

                return View(userMessageVM);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // POST: UserMessageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, UserMessageVM userMessage)
        {
            try
            {
                var dbUserMessage = _context.UserMessages.FirstOrDefault(x => x.UserMessageId == id);
                _context.UserMessages.Remove(dbUserMessage);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
