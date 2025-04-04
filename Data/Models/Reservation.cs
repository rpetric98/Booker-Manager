using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int? PropertyId { get; set; }

    public string? Username { get; set; }

    public DateTime? CheckInDate { get; set; }

    public DateTime? CheckOutDate { get; set; }

    public int? TotalPrice { get; set; }

    public int? NumberOfDays { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Property? Property { get; set; }
}
