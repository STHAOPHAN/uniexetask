﻿using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public string Message { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<GroupInvite> GroupInvites { get; set; } = new List<GroupInvite>();

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
