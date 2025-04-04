using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class PropertyType
{
    public int PropertyTypeId { get; set; }

    public string? PropertyTypeName { get; set; }

    public virtual ICollection<Property> Properties { get; } = new List<Property>();
}
