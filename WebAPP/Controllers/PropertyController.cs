using AutoMapper;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using NuGet.Protocol;
using WebAPP.Models;

namespace WebAPP.Controllers
{
    public class PropertyController : Controller
    {
        private readonly ProjektRwaContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public PropertyController(ProjektRwaContext context, IMapper mapper, IConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
        }

        private List<SelectListItem> GetTypeList()
        {
            var typeListItemsJson = HttpContext.Session.GetString("GenreListItems");

            List<SelectListItem> typeListItems;
            if (typeListItemsJson == null)
            {
                typeListItems = _context.PropertyTypes
                    .Select(x => new SelectListItem
                    {
                        Text = x.PropertyTypeName,
                        Value = x.PropertyTypeId.ToString()
                    }).ToList();

                HttpContext.Session.SetString("TypeListItems", typeListItems.ToJson());
            }
            else
            {
                typeListItems = typeListItemsJson.FromJson<List<SelectListItem>>();
            }

            return typeListItems;
        }

        // GET: PropertyController
        public ActionResult Index(int page = 1, int size = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (size < 1) size = 10;
                IEnumerable<Property> properties = _context.Properties
                    .Include(x => x.PropertyType)
                    .OrderByDescending(x => x.PropertyId);

                var totalCount = properties.Count();
                properties = properties.Skip((page - 1) * size).Take(size);

                var propertyVms = properties.Select(x => new PropertyVM
                { 
                    PropertyId = x.PropertyId,
                    PropertyTypeId = (int)x.PropertyTypeId,
                    PropertyTypeName = x.PropertyType.PropertyTypeName,
                    Name = x.Name,
                    Description = x.Description,
                    Address = x.Address,
                    City = x.City,
                    ZipCode = x.ZipCode,
                    Country = x.Country,
                    PricePerNight = x.PricePerNight,
                    MaxGuests = x.MaxGuests,
                }).ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalCount = totalCount;
                ViewBag.PageSize = size;
                ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)size);
                ViewBag.FromPager = page > 4 ? page - 4 : 1;
                ViewBag.ToPager = page + 4 <= ViewBag.TotalPages ? page + 4 : ViewBag.TotalPages;

                return View(propertyVms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error has ocurred {ex.Message}");
            }
        }

        public ActionResult GetPropertiesByGuest(int? max)
        {
            try
            {
                IQueryable<Property> properties = _context.Properties
                    .Include(x => x.PropertyType);

                if (max != null)
                {
                    properties = properties.Where(x => x.MaxGuests <= max);
                }

                var propertiesVms = properties.Select(x => new PropertyVM
                {
                    PropertyId = x.PropertyId,
                    PropertyTypeId = (int)x.PropertyTypeId,
                    PropertyTypeName = x.PropertyType.PropertyTypeName,
                    Name = x.Name,
                    Description = x.Description,
                    Address = x.Address,
                    City = x.City,
                    ZipCode = x.ZipCode,
                    Country = x.Country,
                    PricePerNight = x.PricePerNight,
                    MaxGuests = x.MaxGuests,

                }).ToList();
                 return View("Index", propertiesVms);   
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult Search(SearchVM searchVM)
        {
            try
            {
                if (string.IsNullOrEmpty(searchVM.X) && string.IsNullOrEmpty(searchVM.Submit))
                {
                    searchVM.X = Request.Cookies["query"];
                }

                Response.Cookies.Append("query", searchVM.X ?? "");

                IQueryable<Property> properties = _context.Properties
                    .Include(x => x.PropertyType)
                    .OrderByDescending(x => x.PropertyId);

                if (!string.IsNullOrEmpty(searchVM.X))
                {
                    properties = properties.Where(x =>
                    x.Name.Contains(searchVM.X) ||
                    x.PropertyType.PropertyTypeName.Contains(searchVM.X));
      
                }

                var filteredCount = properties.Count();
                if (!string.IsNullOrEmpty(searchVM.OrderBy))
                {
                    switch (searchVM.OrderBy.ToLower())
                    {
                        case "id":
                            properties = properties.OrderBy(x => x.PropertyId);
                            break;
                        case "type":
                            properties = properties.OrderBy(x => x.PropertyType.PropertyTypeName);
                            break;
                        case "name":
                            properties = properties.OrderBy(x => x.Name);
                            break;
                        case "address":
                            properties = properties.OrderBy(x => x.Address);
                            break;
                        case "city":
                            properties = properties.OrderBy(x => x.City);
                            break;
                        case "zip":
                            properties = properties.OrderBy(x => x.ZipCode);
                            break;
                        case "country":
                            properties = properties.OrderBy(x => x.Country);
                            break;
                        case "price":
                            properties = properties.OrderBy(x => x.PricePerNight);
                            break;
                        case "guests":
                            properties = properties.OrderBy(x => x.MaxGuests);
                            break;
                        default:
                            break;
                    }
                }

                properties = properties.Skip((searchVM.Page - 1) * searchVM.Size).Take(searchVM.Size);

                searchVM.Properties = properties.Select(x => new PropertyVM
                {
                    PropertyId = x.PropertyId,
                    PropertyTypeId = (int)x.PropertyTypeId,
                    PropertyTypeName = x.PropertyType.PropertyTypeName,
                    Name = x.Name,
                    Description = x.Description,
                    Address = x.Address,
                    City = x.City,
                    ZipCode = x.ZipCode,
                    Country = x.Country,
                    PricePerNight = x.PricePerNight,
                    MaxGuests = x.MaxGuests,
                }).ToList();

                var expandPages = _config.GetValue<int>("Paging:ExpandPages");
                searchVM.LastPage = (int)Math.Ceiling(1.0 * filteredCount / searchVM.Size);
                searchVM.FromPager = searchVM.Page > expandPages ? 
                    searchVM.Page - expandPages : 1;
                searchVM.ToPager = (searchVM.Page + expandPages) < searchVM.LastPage ? searchVM.Page + expandPages : searchVM.LastPage;

                return View(searchVM);
            }   
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public ActionResult SearchPartial(SearchVM searchVM)
        {
            try
            {
                if (string.IsNullOrEmpty(searchVM.X) && string.IsNullOrEmpty(searchVM.Submit))
                {
                    searchVM.X = Request.Cookies["query"];
                }

                Response.Cookies.Append("query", searchVM.X ?? "");

                IQueryable<Property> properties = _context.Properties
                    .Include(x => x.PropertyType)
                    .OrderByDescending(x => x.PropertyId);

                if (!string.IsNullOrEmpty(searchVM.X))
                {
                    properties = properties.Where(x =>
                                       x.Name.Contains(searchVM.X) ||
                                                          x.PropertyType.PropertyTypeName.Contains(searchVM.X));
                }

                var filteredCount = properties.Count();

                if (!string.IsNullOrEmpty(searchVM.OrderBy))
                {
                    switch (searchVM.OrderBy.ToLower())
                    { 
                        case "id":
                            properties = properties.OrderBy(x => x.PropertyId);
                            break;
                        case "type":
                            properties = properties.OrderBy(x => x.PropertyType.PropertyTypeName);
                            break;
                        case "name":
                            properties = properties.OrderBy(x => x.Name);
                            break;
                        case "address":
                            properties = properties.OrderBy(x => x.Address);
                            break;
                        case "city":
                            properties = properties.OrderBy(x => x.City);
                            break;
                        case "zip":
                            properties = properties.OrderBy(x => x.ZipCode);
                            break;
                        case "country":
                            properties = properties.OrderBy(x => x.Country);
                            break;
                        case "price":
                            properties = properties.OrderBy(x => x.PricePerNight);
                            break;
                        case "guest":
                            properties = properties.OrderBy(x => x.MaxGuests);
                            break;
                        default:
                            break;
                    }
                }
                
                properties = properties.Skip((searchVM.Page - 1) * searchVM.Size).Take(searchVM.Size);

                searchVM.Properties = properties.Select(x => new PropertyVM
                {
                    PropertyId = x.PropertyId,
                    PropertyTypeId = (int)x.PropertyTypeId,
                    PropertyTypeName = x.PropertyType.PropertyTypeName,
                    Name = x.Name,
                    Description = x.Description,
                    Address = x.Address,
                    City = x.City,
                    ZipCode = x.ZipCode,
                    Country = x.Country,
                    PricePerNight = x.PricePerNight,
                    MaxGuests = x.MaxGuests,
                }).ToList();

                var expandPages = _config.GetValue<int>("Paging:ExpandPages");
                searchVM.LastPage = (int)Math.Ceiling(1.0 * filteredCount / searchVM.Size);
                searchVM.FromPager = searchVM.Page > expandPages ? searchVM.Page - expandPages : 1;
                searchVM.ToPager = (searchVM.Page + expandPages) < searchVM.LastPage ? searchVM.Page + expandPages : searchVM.LastPage;

                return PartialView("_SearchPartial", searchVM);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        // GET: PropertyController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var property = _context.Properties
                    .Include(x => x.PropertyType)
                    .Include(x => x.PropertyAmenities)
                    .ThenInclude(x => x.Amenity)
                    .FirstOrDefault(x => x.PropertyId == id);

                if (property == null)
                {
                   return NotFound();
                }

                var allAmenities = _context.Amenities.ToList();

                var propertyVM = new PropertyVM
                {
                    PropertyId = property.PropertyId,
                    PropertyTypeId = (int)property.PropertyTypeId,
                    PropertyTypeName = property.PropertyType.PropertyTypeName,
                    Name = property.Name,
                    Description = property.Description,
                    Address = property.Address,
                    City = property.City,
                    ZipCode = property.ZipCode,
                    Country = property.Country,
                    PricePerNight = property.PricePerNight,
                    MaxGuests = property.MaxGuests,

                    AmenityIds = property.PropertyAmenities
                    .Select(x => x.AmenityId)
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .ToList(),

                    Amenities = allAmenities.Select(x => new AmenityVM
                    {
                        AmenityId = x.AmenityId,
                        AmenityName = x.AmenityName
                    }).ToList()
                };

                return View(propertyVM);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        // GET: PropertyController/Create
        public ActionResult Create()
        {
            ViewBag.PropertyTypeDdlItems = GetTypeList();
            var propertyVM = new PropertyVM();

            return View(propertyVM);
        }

        // POST: PropertyController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PropertyVM property)
        {
            var trimmedProperty = property.Name.Trim();
            if (_context.Properties.Any(x => x.Name.Equals(trimmedProperty)))
            {
                ModelState.AddModelError("", "This property already exists");
                return View();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.PropertyTypeDdlItems = GetTypeList();

                    ModelState.AddModelError("", "Failed to create property");

                    return View(property);
                }

                var newProperty = new Property
                {
                    PropertyId = property.PropertyId,
                    PropertyTypeId = property.PropertyTypeId,
                    Name = property.Name,
                    Description = property.Description,
                    Address = property.Address,
                    City = property.City,
                    ZipCode = property.ZipCode,
                    Country = property.Country,
                    PricePerNight = property.PricePerNight,
                    MaxGuests = property.MaxGuests
                };

                _context.Properties.Add(newProperty);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the property: " + ex.Message);
                ViewBag.PropertyTypeDdlItems = GetTypeList();
                return View(property);
            }
        }
    

        private IQueryable<SelectListItem> GetPropertyTypes()
        { 
            return _context.PropertyTypes
                .Select(x => new SelectListItem
                {
                    Value = x.PropertyTypeId.ToString(),
                    Text = x.PropertyTypeName
                });
        }

        private IQueryable<SelectListItem> GetUserDetails()
        { 
            return _context.UserDetails.Select(x => new SelectListItem
            {
                    Value = x.UserDetailId.ToString(),
                    Text = x.FirstName + " " + x.LastName
                });
        }

        private List<SelectListItem> GetAmenityListItems()
        { 
            return _context.Amenities
                .Select(x => new SelectListItem
                {
                    Value = x.AmenityId.ToString(),
                    Text = x.AmenityName
                }).ToList();
        }

        // GET: PropertyController/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.PropertyTypeDdlItems = GetPropertyTypes();
            ViewBag.AmenityDdlItems = GetAmenityListItems();

            var property = _context.Properties
                .Include(x => x.PropertyAmenities)
                .FirstOrDefault(x => x.PropertyId == id);

            var propertyVM = new PropertyVM
            { 
                PropertyId = property.PropertyId,
                PropertyTypeId = property.PropertyTypeId,
                Name = property.Name,
                Description = property.Description,
                Address = property.Address,
                City = property.City,
                ZipCode = property.ZipCode,
                Country = property.Country,
                PricePerNight = property.PricePerNight,
                MaxGuests = property.MaxGuests,
                AmenityIds = property.PropertyAmenities
                    .Select(x => x.AmenityId)
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .ToList()
            };

            return View(propertyVM);
        }

        // POST: PropertyController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PropertyVM property)
        {
            try
            {
                var dbProperty = _context.Properties.Include(x => x.PropertyAmenities).FirstOrDefault(x => x.PropertyId == id);
                dbProperty.PropertyTypeId = property.PropertyTypeId;
                dbProperty.Name = property.Name;
                dbProperty.Description = property.Description;
                dbProperty.Address = property.Address;
                dbProperty.City = property.City;
                dbProperty.ZipCode = property.ZipCode;
                dbProperty.Country = property.Country;
                dbProperty.PricePerNight = property.PricePerNight;
                dbProperty.MaxGuests = property.MaxGuests;

                _context.RemoveRange(dbProperty.PropertyAmenities);
                var propertyAmenities = property.AmenityIds.Select(x => new PropertyAmenity { PropertyId = id, AmenityId = x });
                dbProperty.PropertyAmenities.AddRange(propertyAmenities);

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PropertyController/Delete/5
        public ActionResult Delete(int id)
        {
            var property = _context.Properties
                .Include(x => x.PropertyType)
                .FirstOrDefault(x => x.PropertyId == id);

            var propertyVM = new PropertyVM
            {
                PropertyId = property.PropertyId,
                PropertyTypeId = (int)property.PropertyTypeId,
                Name = property.Name,
                Description = property.Description,
                Address = property.Address,
                City = property.City,
                ZipCode = property.ZipCode,
                Country = property.Country,
                PricePerNight = property.PricePerNight,
                MaxGuests = property.MaxGuests
            };

            return View(propertyVM);

        }

        // POST: PropertyController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var dbProperties = _context.Properties.FirstOrDefault(x => x.PropertyId == id);
                _context.Properties.Remove(dbProperties);
                _context.SaveChanges();
                return RedirectToAction("Index", "Property");
            }
            catch
            {
                return View();
            }
        }
    }
}
