using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class PropertyAmenity
{
    public int PropertyAmenityId { get; set; }

    public int? PropertyId { get; set; }

    public int? AmenityId { get; set; }

    public virtual Amenity? Amenity { get; set; }

    public virtual Property? Property { get; set; }
}
