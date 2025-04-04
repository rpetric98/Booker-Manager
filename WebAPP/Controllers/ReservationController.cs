using AutoMapper.Configuration.Conventions;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPP.Models;

namespace WebAPP.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly ProjektRwaContext _context;
        private readonly IConfiguration _config;

        public ReservationController(ProjektRwaContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }



        // GET: ReservationController
        public ActionResult Index(int page = 1, int size = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (size < 1) size = 10;

                IEnumerable<Reservation> reservations = _context.Reservations
                    .Include(r => r.Property)
                    .OrderByDescending(r => r.ReservationId);

                var totalCount = reservations.Count();

                reservations = reservations.Skip((page - 1) * size).Take(size);

                var reservationVMS = reservations.Select(r => new ReservationVM
                {
                    ReservationID = r.ReservationId,
                    PropertyID = (int)r.PropertyId,
                    PropertyName = r.Property.Name,
                    Username = r.Username,
                    CheckIn = r.CheckInDate,
                    CheckOut = r.CheckOutDate,
                    NumberOfDays = (int)r.NumberOfDays,
                    TotalPrice = r.TotalPrice
                }).ToList();

                ViewBag.CurrentPage = page;
                ViewBag.PageSize = size;
                ViewBag.TotalCount = totalCount;
                ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)size);
                ViewBag.FromPager = page > 4 ? page - 4 : 1;
                ViewBag.ToPager = page + 4 <= ViewBag.TotalPages ? page + 4 : ViewBag.TotalPages;

                return View(reservationVMS);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        public ActionResult UserIndex(ReservationUserVM reservationUserVM)
        {
            try
            {
                if (string.IsNullOrEmpty(reservationUserVM.Username) && string.IsNullOrEmpty(reservationUserVM.Submit))
                {
                    reservationUserVM.Username = Request.Cookies["query"];
                }

                Response.Cookies.Append("query", reservationUserVM.Username ?? "");

                IQueryable<Reservation> reservations = _context.Reservations
                    .Include(r => r.Property)
                    .OrderByDescending(r => r.ReservationId);

                if (!string.IsNullOrEmpty(reservationUserVM.Username))
                { 
                    reservations = reservations.Where(r =>
                    r.Username.Contains(reservationUserVM.Username));
                }

                var filteredCount = reservations.Count();

                reservations = reservations.Skip((reservationUserVM.Page - 1) * reservationUserVM.Size).Take(reservationUserVM.Size);
                
                reservationUserVM.Reservations = reservations.Select(r => new ReservationVM
                {
                    ReservationID = r.ReservationId,
                    PropertyID = (int)r.PropertyId,
                    PropertyName = r.Property.Name,
                    Username = r.Username,
                    CheckIn = r.CheckInDate,
                    CheckOut = r.CheckOutDate,
                    NumberOfDays = (int)r.NumberOfDays,
                    TotalPrice = r.TotalPrice
                }).ToList();

                var expandPages = _config.GetValue<int>("Paging:ExpandPages");
                reservationUserVM.LastPage = (int)Math.Ceiling(1.0 * filteredCount / reservationUserVM.Size);
                reservationUserVM.FromPager = reservationUserVM.Page > expandPages ? reservationUserVM.Page - expandPages : 1;
                reservationUserVM.ToPager = (reservationUserVM.Page + expandPages) < reservationUserVM.LastPage ? reservationUserVM.Page + expandPages : reservationUserVM.LastPage;
                return View(reservationUserVM);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        // GET: ReservationController/Details/5
        public ActionResult Details(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Property)
                .FirstOrDefault(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var reservationVM = new ReservationVM
            { 
                ReservationID = reservation.ReservationId,
                PropertyName = reservation.Property.Name,
                Username = reservation.Username,
                CheckIn = reservation.CheckInDate,
                CheckOut = reservation.CheckOutDate,
                NumberOfDays = (int)reservation.NumberOfDays,
                TotalPrice = reservation.TotalPrice
            };
            

            return View(reservationVM);
        }

        // GET: ReservationController/Create
        public ActionResult Create(int propertyId)
        {
            var property = _context.Properties.Find(propertyId);
            if (property == null)
            {
                return NotFound();
            }

            var reservationVM = new ReservationVM
            { 
                PropertyID = propertyId,
                PropertyName = property.Name,
            };

            return View(reservationVM);
        }

        // POST: ReservationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReservationVM reservationVM)
        {
            var property = _context.Properties.Find(reservationVM.PropertyID);
            if (property == null)
            {
                return NotFound();
            }

            if (reservationVM.CheckIn == null || reservationVM.CheckOut == null)
            { 
                ModelState.AddModelError("", "Check In and Check Out are required");
                return View(reservationVM);
            }

            TimeSpan? difference = reservationVM.CheckOut - reservationVM.CheckIn;
            if(difference.HasValue && difference.Value.Days > 0)
            { 
                int numberOfDays = difference.Value.Days;
                var existingReservations = _context.Reservations
                    .Where(r => r.PropertyId == reservationVM.PropertyID)
                    .Where(r => r.CheckInDate <= reservationVM.CheckOut.Value && r.CheckOutDate >= reservationVM.CheckIn.Value)
                    .ToList();

                if (existingReservations.Any())
                {
                    ModelState.AddModelError("", "Property is already reserved for selected dates");
                    return View(reservationVM);
                }

                var reservation = new Reservation
                {
                    PropertyId = reservationVM.PropertyID,
                    Username = reservationVM.Username,
                    CheckInDate = reservationVM.CheckIn.Value,
                    CheckOutDate = reservationVM.CheckOut.Value,
                    NumberOfDays = numberOfDays,
                    TotalPrice = numberOfDays * property.PricePerNight
                };

                if (ModelState.IsValid)
                {
                    _context.Reservations.Add(reservation);
                    _context.SaveChanges();

                    return RedirectToAction("Confirmation", new
                    { 
                        propertyId = reservation.PropertyId,
                        username = reservation.Username,
                        checkIn = reservation.CheckInDate,
                        checkOut = reservation.CheckOutDate,
                        numberOfDays = reservation.NumberOfDays,
                        totalPrice = reservation.TotalPrice
                    });
                }
            }
            else
            {
                ModelState.AddModelError("", "Check Out must be greater than Check In");
                return View(reservationVM);
            }
            return View(reservationVM);
        }

        public IActionResult Confirmation(int propertyId, string username, DateTime checkIn, DateTime checkOut, int numberOfDays, int totalPrice)
        { 
            var property = _context.Properties.Find(propertyId);
            if (property == null)
            {
                return NotFound();
            }

            var reservationVm = new ReservationVM
            {
                PropertyID = propertyId,
                PropertyName = property.Name,
                Username = username,
                CheckIn = checkIn,
                CheckOut = checkOut,
                NumberOfDays = numberOfDays,
                TotalPrice = totalPrice
            };

            return View(reservationVm);
        }

        // GET: ReservationController/Edit/5
        public ActionResult Edit(int id)
        {
            var reservation = _context.Reservations.Include(r => r.Property).FirstOrDefault(r => r.ReservationId == id);

            var reservationVM = new ReservationVM
            {
                ReservationID = reservation.ReservationId,
                PropertyID = (int)reservation.PropertyId,
                PropertyName = reservation.Property.Name,
                Username = reservation.Username,
                CheckIn = reservation.CheckInDate,
                CheckOut = reservation.CheckOutDate,
                NumberOfDays = (int)reservation.NumberOfDays,
                TotalPrice = reservation.TotalPrice
            };

            return View(reservationVM);
        }

        // POST: ReservationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ReservationVM reservation)
        {
            try
            {
                var dbReservation = _context.Reservations
                    .Include(x => x.Property)
                    .FirstOrDefault(x => x.ReservationId == id);

                if (dbReservation == null)
                {
                    return NotFound();                
                }

                if (reservation.CheckIn == null || reservation.CheckOut == null)
                {
                    ModelState.AddModelError("", "Check In and Check Out are required");
                    return View(reservation);
                }

                TimeSpan? difference = reservation.CheckOut - reservation.CheckIn;

                if (difference.HasValue && difference.Value.Days > 0)
                {
                    var overlappingReservations = _context.Reservations
                        .Where(r => r.ReservationId != id && r.PropertyId == dbReservation.PropertyId &&
                        ((r.CheckInDate <= reservation.CheckOut && r.CheckOutDate >= reservation.CheckIn)))
                        .ToList();
                    if (overlappingReservations.Any())
                    {
                        ModelState.AddModelError("", "The selected dates are already booked");
                        return View(reservation);
                    }

                    dbReservation.Username = reservation.Username;
                    dbReservation.CheckInDate = reservation.CheckIn.Value;
                    dbReservation.CheckOutDate = reservation.CheckOut.Value;
                    dbReservation.NumberOfDays = difference.Value.Days;
                    dbReservation.TotalPrice = difference.Value.Days * dbReservation.Property.PricePerNight;

                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Check Out must be greater than Check In");
                    return View(reservation);
                }
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while updating the reservation");
                return View(reservation);
            }
        }

        // GET: ReservationController/Delete/5
        public ActionResult Delete(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Property)
                .FirstOrDefault(r => r.ReservationId == id);

            var reservationVM = new ReservationVM
            {
                ReservationID = reservation.ReservationId,
                PropertyID = (int)reservation.PropertyId,
                PropertyName = reservation.Property.Name,
                Username = reservation.Username,
                CheckIn = reservation.CheckInDate,
                CheckOut = reservation.CheckOutDate,
                NumberOfDays = (int)reservation.NumberOfDays,
                TotalPrice = reservation.TotalPrice
            };

            return View(reservationVM);
        }

        // POST: ReservationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var reservation = _context.Reservations.FirstOrDefault(r => r.ReservationId == id);
                _context.Reservations.Remove(reservation);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }
    }
}
