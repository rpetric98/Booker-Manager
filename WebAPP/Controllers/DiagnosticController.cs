using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPP.Models;
using WebAPP.Services;

namespace WebAPP.Controllers
{
    public class DiagnosticController : Controller
    {
        private readonly ProjektRwaContext _context;
        private readonly Diagnostics _diagnostics;

        public DiagnosticController(ProjektRwaContext context, Diagnostics diagnostics)
        {
            _context = context;
            _diagnostics = diagnostics;
        }

        public IActionResult Index()
        {
            var diagnostics = new DiagnosticsVM
            {
                PathFileCount = _diagnostics.CountPathFiles(),
                PropertyCount = _diagnostics.CountProperties()
            };
            return View(diagnostics);
        }
    }
}
