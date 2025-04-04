using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using WebAPP.Models;
using WebAPP.Services;


namespace WebAPP.Controllers
{
    public class PropertyAmenityController : Controller
    {
        private readonly ProjektRwaContext _context;

        public PropertyAmenityController(ProjektRwaContext context)
        {
            _context = context;
        }

        // GET: PropertyAmenityController
        public IActionResult Index()
        {
            try
            {
                var propertyAmenityVms = _context.PropertyAmenities
                    .Include(x => x.Property)
                    .Include(x => x.Amenity)
                    .Select(x => new PropertyAmenityVM
                    {
                        PropertyAmenityId = x.PropertyAmenityId,
                        PropertyId = x.PropertyAmenityId,
                        PropertyName = x.Property.Name,
                        AmenityId = x.Amenity.AmenityId,
                        AmenityName = x.Amenity.AmenityName
                    }).ToList();

                return View(propertyAmenityVms);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: PropertyAmenityController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var propertyAmenity = _context.PropertyAmenities
                    .Include(x => x.Property)
                    .Include(x => x.Amenity)
                    .FirstOrDefault(x => x.PropertyAmenityId == id);

                if (propertyAmenity == null)
                {
                    return NotFound();
                }

                var propertyAmenityVM = new PropertyAmenityVM
                {
                    PropertyAmenityId = propertyAmenity.PropertyAmenityId,
                    PropertyId = (int)propertyAmenity.PropertyId,
                    PropertyName = propertyAmenity.Property.Name,
                    AmenityId = (int)propertyAmenity.AmenityId,
                    AmenityName = propertyAmenity.Amenity.AmenityName
                };

                return View(propertyAmenityVM);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public IQueryable<SelectListItem> GetAmenity()
        {
            return _context.Amenities.Select(a => new SelectListItem
            {
                Value = a.AmenityId.ToString(),
                Text = a.AmenityName
            });
        }

        public List<SelectListItem> GetAmenityList()
        {
            var amenityList = HttpContext.Session.GetString("AmenityListItems");
            List<SelectListItem> amenityListItems;
            if (amenityList == null)
            {
                amenityListItems = _context.Amenities
                    .Select(x => new SelectListItem
                    {
                        Value = x.AmenityId.ToString(),
                        Text = x.AmenityName
                    }).ToList();
                HttpContext.Session.SetString("AmenityListItems", amenityList.ToJson());
            }
            else
            {
                amenityListItems = amenityList.FromJson<List<SelectListItem>>();
            }

            return amenityListItems;

        }

        public IQueryable<SelectListItem> GetProperty()
        {
            return _context.Properties.Select(x =>
            new SelectListItem
            {
                Value = x.PropertyId.ToString(),
                Text = x.Name
            });
        }

        public List<SelectListItem> GetPropertyList()
        {
            var propertyList = HttpContext.Session.GetString("PropertyListItems");
            List<SelectListItem> propertyListItems;
            if (propertyList == null)
            {
                propertyListItems = _context.Properties
                    .Select(x => new SelectListItem
                    {
                        Value = x.PropertyId.ToString(),
                        Text = x.Name
                    }).ToList();
                HttpContext.Session.SetString("PropertyListItems", propertyList.ToJson());
            }
            else
            {
                propertyListItems = propertyList.FromJson<List<SelectListItem>>();
            }

            return propertyListItems;
        }
    

    // GET: PropertyAmenityController/Create
    public ActionResult Create()
        {
            ViewBag.AmenityDdlItems = GetAmenityList();
            ViewBag.PropertyDdlItems = GetPropertyList();

            var propertyAmenity = new PropertyAmenityVM();

            return View(propertyAmenity);
        }

        // POST: PropertyAmenityController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PropertyAmenityVM propertyAmenityVM)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.PropertyDdlItems = GetPropertyList();
                ViewBag.AmenityDdlItems = GetAmenityList();
                ModelState.AddModelError("", "Failed to create this");
                return View();
            }

            var exists = _context.PropertyAmenities
           .Any(x => x.PropertyId == propertyAmenityVM.PropertyId && x.AmenityId == propertyAmenityVM.AmenityId);

            if (exists)
            {
                ModelState.AddModelError("", "This amenity is already assigned to the property.");
                ViewBag.PropertyDdlItems = GetPropertyList();
                ViewBag.AmenityDdlItems = GetAmenityList();
                return View(propertyAmenityVM);
            }

            try
            {
                var newPropertyAmenity = new PropertyAmenity
                {
                    PropertyId = propertyAmenityVM.PropertyId,
                    AmenityId = propertyAmenityVM.AmenityId
                };

                _context.PropertyAmenities.Add(newPropertyAmenity);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the property: " + ex.Message);

                ViewBag.PropertyDdlItems = GetPropertyList();
                ViewBag.AmenityDdlItems = GetAmenityList();

                return View(propertyAmenityVM);
            }
        }

        // GET: PropertyAmenityController/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.PropertyDdlItems = GetPropertyList();
            ViewBag.AmenityDdlItems = GetAmenityList();

            var propertyAmenity = _context.PropertyAmenities
                .Include(x => x.Property)
                .Include(x => x.Amenity)
                .FirstOrDefault(x => x.PropertyAmenityId == id);

            if (propertyAmenity == null)
                { return NotFound(); }

            var propertyAmenityVM = new PropertyAmenityVM
            {
                PropertyAmenityId = propertyAmenity.PropertyAmenityId,
                PropertyId = (int)propertyAmenity.PropertyId,
                AmenityId = (int)propertyAmenity.AmenityId
            };

            return View(propertyAmenityVM);
        }

        // POST: PropertyAmenityController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PropertyAmenityVM propertyAmenityVM)
        {
            try
            {
                var propertyAmenity = _context.PropertyAmenities
                    .Include(x => x.Property)
                    .Include(x => x.Amenity)
                    .FirstOrDefault(x => x.PropertyAmenityId == id);

                propertyAmenity.PropertyId = propertyAmenityVM.PropertyId;
                propertyAmenity.AmenityId = propertyAmenityVM.AmenityId;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PropertyAmenityController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var propertyAmenity = _context.PropertyAmenities
                    .Include(x => x.Property)
                    .Include(x => x.Amenity)
                    .FirstOrDefault(x => x.PropertyAmenityId == id);

                if (propertyAmenity == null)

                { return NotFound(); } 

                var propertyAmenityVM = new PropertyAmenityVM
                {
                    PropertyAmenityId = propertyAmenity.PropertyAmenityId,
                    PropertyId = (int)propertyAmenity.PropertyId,
                    PropertyName = propertyAmenity.Property.Name,
                    AmenityId = (int)propertyAmenity.AmenityId,
                    AmenityName = propertyAmenity.Amenity.AmenityName
                };

                return View(propertyAmenityVM);
     
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        // POST: PropertyAmenityController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var propertyAmenity = _context.PropertyAmenities.Find(id);
                if (propertyAmenity != null)
                {
                    _context.PropertyAmenities.Remove(propertyAmenity);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
