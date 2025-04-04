using Data.Models;

namespace WebAPP.Services
{
    public class Diagnostics : IDiagnostics
    {
        private readonly ProjektRwaContext _context;

        public Diagnostics(ProjektRwaContext context)
        {
            _context = context;
        }

        public int CountPathFiles()
        {
            var pathFiles = Path.GetTempPath();
            return Directory.GetFiles(pathFiles).Length;
        }

        public int CountProperties()
        {
            return _context.Properties.Count();
        }


    }
}
