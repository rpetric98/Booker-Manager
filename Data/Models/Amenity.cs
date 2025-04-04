using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Amenity
{
    public int AmenityId { get; set; }

    public string? AmenityName { get; set; }

    public virtual ICollection<PropertyAmenity> PropertyAmenities { get; } = new List<PropertyAmenity>();
}
