﻿using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public int ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}