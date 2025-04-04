using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Dtos;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly ProjektRwaContext _context;
        private readonly ILogService _logService;

        public PropertyController(ProjektRwaContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Property>> GetProperties()
        {
            try
            {
                var result = _context.Properties;
                var mappedResult = result.Select(x =>
                new PropertyDto
                {
                    PropertyId =  x.PropertyId,
                    PropertyTypeName = x.PropertyType.PropertyTypeName,
                    Name = x.Name,
                    Description = x.Description,
                    Address = x.Address,
                    City = x.City,
                    ZipCode = x.ZipCode,
                    Country = x.Country,
                    PricePerNight = (int)x.PricePerNight,
                    MaxGuests = (int)x.MaxGuests

                });
                _logService.Log("Info", "Properties retrieved");
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                _logService.Log("Error", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]/{id}")]
        public ActionResult<PropertyDto> GetPropertyById(int id)
        {
            try
            {
                var result = _context.Properties
                    .Include(x => x.PropertyType)
                    .Include(x => x.PropertyAmenities)
                    .FirstOrDefault(x => x.PropertyId == id);
                if (result == null)
                {
                    _logService.Log("Error", $"Property with id={id} is not found.");
                    return NotFound("Property not found");
                }

                var mappedResult = new PropertyDto
                {
                    PropertyId = result.PropertyId,
                    PropertyTypeName = result.PropertyType.PropertyTypeName,
                    Name = result.Name,
                    Description = result.Description,
                    Address = result.Address,
                    City = result.City,
                    ZipCode = result.ZipCode,
                    Country = result.Country,
                    PricePerNight = (int)result.PricePerNight,
                    MaxGuests = (int)result.MaxGuests
                };
                _logService.Log("Info", $"Property with id={id} retrieved");
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                _logService.Log("Error", $"Error while retrieving {id}: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult<IEnumerable<Property>> Search(string text, string sortType, int page = 1, int count = 10)
        {
            try
            {
                if (page < 1)
                {
                    page = 1;
                }
                if (count < 1)
                {
                    count = 10;
                }

                IEnumerable<Property> result = _context.Properties.Include(x => x.PropertyType);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    result = result.Where(x => x.Name.Contains(text, StringComparison.OrdinalIgnoreCase));
                }

                switch (sortType)
                {
                    case "Id":
                        result = result.OrderBy(x => x.PropertyId);
                        break;
                        case "Name":
                            result = result.OrderBy(x => x.Name);
                        break;
                    default:
                        _logService.Log("Error", $"Invalid sort type: {sortType}");
                        break;
                }

                result = result.Skip((page - 1) * count).Take(count);
                _logService.Log("Info", $"Search for {text} with sort type {sortType} returned {result.Count()} results");
                return Ok(result);
            }
            catch (Exception)
            {
                _logService.Log("Error", "Error while searching properties");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("[action]")]
        public ActionResult<PropertyDto> NewProperty([FromBody] PropertyDto value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest("There is no value.");
                }

                // Validate ModelState
                if (!ModelState.IsValid)
                {
                    _logService.Log("ERROR", "Model state is invalid.");
                    return BadRequest(ModelState);
                }

                // Ensure PropertyTypeName is provided
                if (string.IsNullOrWhiteSpace(value.PropertyTypeName))
                {
                    return BadRequest("Property type name is required.");
                }

                var trimmedPropertyTypeName = value.PropertyTypeName.Trim();

                var propertyType = _context.PropertyTypes.FirstOrDefault(x => x.PropertyTypeName == trimmedPropertyTypeName);

                if (propertyType == null)
                {
                    propertyType = new PropertyType
                    {
                        PropertyTypeName = trimmedPropertyTypeName
                    };

                    _context.PropertyTypes.Add(propertyType);
                    _context.SaveChanges(); // Save the new PropertyType to get its ID
                }

                var trimmedPropertyName = value.Name.Trim();

                var property = _context.Properties
                    .FirstOrDefault(x => x.Name == trimmedPropertyName);

                if (property != null)
                {
                    return BadRequest("Property name is required");
                }

                var newProperty = new Property
                {
                    PropertyTypeId = propertyType.PropertyTypeId,
                    Name = value.Name,
                    Description = value.Description,
                    Address = value.Address,
                    City = value.City,
                    ZipCode = value.ZipCode,
                    Country = value.Country,
                    PricePerNight = value.PricePerNight,
                    MaxGuests = value.MaxGuests
                };

                _context.Properties.Add(newProperty);
                _context.SaveChanges();

                value.PropertyId = newProperty.PropertyId;

                _logService.Log("INFO", $"Property entity was created where id={newProperty.PropertyId}.");
                return Ok(value);
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"There was a problem creating Property entity: {ex.Message}");
                return BadRequest("An error occurred while creating the property.");
            }
        }

        [HttpPut("[action]/{id}")]
        public ActionResult<PropertyDto> UpdateProperty(int id, [FromBody] PropertyDto value)
        {
            try
            {
                var existingProperty = _context.Properties.FirstOrDefault(x => x.PropertyId == id);
                if (existingProperty == null)
                {
                    _logService.Log("ERROR", $"Could not find Property where id={id}.");
                    return NotFound("Property not found.");
                }

                if (string.IsNullOrWhiteSpace(value.PropertyTypeName))
                {
                    return BadRequest("Property type name is required.");
                }

                var trimmedPropertyTypeName = value.PropertyTypeName.Trim();

                var propertyType = _context.PropertyTypes
                    .FirstOrDefault(x => x.PropertyTypeName == trimmedPropertyTypeName);

                if (propertyType == null)
                {
                    _logService.Log("ERROR", $"Property type with name '{trimmedPropertyTypeName}' does not exist.");
                    return BadRequest($"Property type '{trimmedPropertyTypeName}' does not exist.");
                }

                var trimmedPropertyName = value.Name.Trim();

                var property = _context.Properties
                    .FirstOrDefault(x => x.Name == trimmedPropertyName);

                if (property != null)
                {
                    return BadRequest("Property name is required");
                }

                existingProperty.PropertyTypeId = propertyType.PropertyTypeId;
                existingProperty.Name = value.Name;
                existingProperty.Name = value.Name;
                existingProperty.Description = value.Description;
                existingProperty.Address = value.Address;
                existingProperty.City = value.City;
                existingProperty.ZipCode = value.ZipCode;
                existingProperty.Country = value.Country;
                existingProperty.PricePerNight = value.PricePerNight;
                existingProperty.MaxGuests = value.MaxGuests;

                _context.SaveChanges();

                var updatedPropertyDto = new PropertyDto
                {
                    PropertyId = existingProperty.PropertyId,
                    PropertyTypeName = propertyType.PropertyTypeName,
                    Name = existingProperty.Name,
                    Description = existingProperty.Description,
                    Address = existingProperty.Address,
                    City = existingProperty.City,
                    ZipCode = existingProperty.ZipCode,
                    Country = existingProperty.Country,
                    PricePerNight = existingProperty.PricePerNight,
                    MaxGuests = existingProperty.MaxGuests
                };

                _logService.Log("INFO", $"Property entity was updated where id={id}.");

                return Ok(updatedPropertyDto);
            }
            catch (Exception ex)
            {
                _logService.Log("ERROR", $"There was a problem with updating Property where id={id}: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("[action]/{id}")]
        public ActionResult<PropertyDto> DeleteProperty(int id)
        {
            try
            {
                var existingProperty = _context.Properties.FirstOrDefault(x => x.PropertyId == id);
                if (existingProperty == null)
                {
                    _logService.Log("Error", $"Property with id={id} is not found.");
                    return NotFound("Property not found");
                }

                _context.Properties.Remove(existingProperty);
                _context.SaveChanges();
                _logService.Log("Info", $"Property with id={id} deleted");
                return Ok();
            }
            catch (Exception ex)
            {
                _logService.Log("Error", $"Error while deleting {id}: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }   
    }
}
