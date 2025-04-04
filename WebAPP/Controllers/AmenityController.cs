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
    public class AmenityController : Controller
    {
        private readonly ProjektRwaContext _context;
        private readonly IMapper _mapper;

        public AmenityController(ProjektRwaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // GET: AmenityController
        public ActionResult Index()
        {
            try
            {
                AmenityVM amenity = null;
                if (TempData.ContainsKey("newGenre"))
                {
                    amenity = ((string)TempData["newAmenity"]).FromJson<AmenityVM>();
                }

                var amenityVMS = _context.Amenities.Select(x => new AmenityVM
                { 
                    AmenityId = x.AmenityId,
                    AmenityName = x.AmenityName
                }).ToList();

                var amenities = _context.Amenities;
                var amenityVM = _mapper.Map<List<AmenityVM>>(amenities);

                return View(amenityVM);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        // GET: AmenityController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var amenity = _context.Amenities.FirstOrDefault(x => x.AmenityId == id);
                var amenitiyVM = new AmenityVM
                {
                    AmenityId = amenity.AmenityId,
                    AmenityName = amenity.AmenityName
                };

                return View(amenitiyVM);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return View();
        }

        // GET: AmenityController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AmenityController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AmenityVM amenity)
        {
            try
            {
                
                    var amenityModel = new Amenity
                    {
                        AmenityName = amenity.AmenityName
                    };

                    _context.Amenities.Add(amenityModel);
                    _context.SaveChanges();

                    TempData["newAmenity"] = amenity.ToJson();
                    return RedirectToAction(nameof(Index));
                
            }
            catch
            {
                return View();
            }
        }

        // GET: AmenityController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var amenity = _context.Amenities.FirstOrDefault(x => x.AmenityId == id);
                var amenityVM = new AmenityVM
                {
                    AmenityId = amenity.AmenityId,
                    AmenityName = amenity.AmenityName
                };
                return View(amenityVM);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        // POST: AmenityController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AmenityVM amenity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var amenityModel = _context.Amenities.FirstOrDefault(x => x.AmenityId == id);
                    amenityModel.AmenityName = amenity.AmenityName;

                    _context.Amenities.Update(amenityModel);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: AmenityController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var amenity = _context.Amenities.FirstOrDefault(x => x.AmenityId == id);
                var amenityVM = new AmenityVM
                {
                    AmenityId = amenity.AmenityId,
                    AmenityName = amenity.AmenityName
                };

                return View(amenityVM);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
                
            }
        }

        // POST: AmenityController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, AmenityVM amenity)
        {
            try
            {
                var amenityModel = _context.Amenities.FirstOrDefault(x => x.AmenityId == id);
                _context.Amenities.Remove(amenityModel);
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
