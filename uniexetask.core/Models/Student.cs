﻿using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public int UserId { get; set; }

    public string StudentCode { get; set; } = null!;

    public string Major { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual User User { get; set; } = null!;
}