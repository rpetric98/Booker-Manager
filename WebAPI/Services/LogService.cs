using Data.Models;

namespace WebAPI.Services
{
    public interface ILogService
    {
        void Log(string level, string message);
    }
    public class LogService : ILogService
    {
        private readonly ProjektRwaContext _context;
        public LogService(ProjektRwaContext context)
        {
            _context = context;
        }

        public void Log(string level, string message)
        { 
            var log = new Log
            {
                Level = level,
                Message = message,
            };
            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }
}
