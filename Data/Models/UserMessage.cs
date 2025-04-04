using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class UserMessage
{
    public int UserMessageId { get; set; }

    public string? Username { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedAt { get; set; }
}
