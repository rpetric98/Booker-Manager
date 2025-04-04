using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ProjektRwaContext _context;

        public LogController(ProjektRwaContext context)
        {
            _context = context;
        }

        [HttpGet("get/{X:int?}")]
        public ActionResult<IEnumerable<Log>> GetLogs(int x = 10)
        {
            try
            {
                var logs = _context.Logs
                    .Take(x)
                    .ToList();

                return Ok(logs);
            }
            catch (Exception ex)
            { 
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("count")]
        public ActionResult<int> GetCount()
        {
            try
            { 
                var count = _context.Logs.Count();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
