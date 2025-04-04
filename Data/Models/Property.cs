using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Property
{
    public int PropertyId { get; set; }

    public int? PropertyTypeId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? ZipCode { get; set; }

    public string? Country { get; set; }

    public int? PricePerNight { get; set; }

    public int? MaxGuests { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<PropertyAmenity> PropertyAmenities { get; } = new List<PropertyAmenity>();

    public virtual PropertyType? PropertyType { get; set; }

    public virtual ICollection<Reservation> Reservations { get; } = new List<Reservation>();
}
