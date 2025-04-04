using AutoMapper;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using WebAPP.Models;

namespace WebAPP.Controllers
{
    [Authorize]
    public class PropertyTypeController : Controller
    {
        private readonly ProjektRwaContext _context;
        private readonly IMapper _mapper;

        public PropertyTypeController(ProjektRwaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: PropertyTypeController
        public ActionResult Index()
        {
            try
            {
                PropertyTypeVM newPopertyType = null;
                if (TempData.ContainsKey("newGenre"))
                {
                    newPopertyType = ((string)TempData["newGenre"]).FromJson<PropertyTypeVM>();
                }

                var propertyTypes = _context.PropertyTypes;
                var propertyTypesVM = _mapper.Map<IEnumerable<PropertyTypeVM>>(propertyTypes);

                return View(propertyTypesVM);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        // GET: PropertyTypeController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var propertyType = _context.PropertyTypes.FirstOrDefault(x => x.PropertyTypeId == id);
                var propertyTypeVM = _mapper.Map<IEnumerable<PropertyTypeVM>>(propertyType);
                return View(propertyTypeVM);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        // GET: PropertyTypeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropertyTypeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PropertyTypeVM propertyType)
        {
            var trimmedPropertyType = propertyType.PropertyTypeName.Trim();
            if(_context.PropertyTypes.Any(x => x.PropertyTypeName.Equals(trimmedPropertyType)))
            {
                ModelState.AddModelError("", "Property type already exists");
                return View();
            }

            try
            {
                var newPropertyType = new PropertyType
                {
                    PropertyTypeName = propertyType.PropertyTypeName
                };

                _context.PropertyTypes.Add(newPropertyType);
                _context.SaveChanges();

                TempData["newPropertyType"] = newPropertyType.ToJson();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PropertyTypeController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var propertyType = _context.PropertyTypes.FirstOrDefault(x => x.PropertyTypeId == id);
                var propertyTypeVM = new PropertyTypeVM
                {
                    PropertyTypeId = propertyType.PropertyTypeId,
                    PropertyTypeName = propertyType.PropertyTypeName
                };

                return View(propertyTypeVM);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        // POST: PropertyTypeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PropertyTypeVM propertyType)
        {
            try
            {
                var dbPropertyType = _context.PropertyTypes.FirstOrDefault(x => x.PropertyTypeId == id);
                dbPropertyType.PropertyTypeName = propertyType.PropertyTypeName;

                _context.SaveChanges();
                
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: PropertyTypeController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var propertyType = _context.PropertyTypes.FirstOrDefault(x => x.PropertyTypeId == id);
                var propertyTypeVM = new PropertyTypeVM
                {
                    PropertyTypeId = propertyType.PropertyTypeId,
                    PropertyTypeName = propertyType.PropertyTypeName
                };
                return View(propertyTypeVM);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // POST: PropertyTypeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, PropertyTypeVM propertyType)
        {
            try
            {
                var dbPropertyType = _context.PropertyTypes.FirstOrDefault(x => x.PropertyTypeId == id);
                _context.PropertyTypes.Remove(dbPropertyType);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
