﻿using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Permission
{
    public int PermissionId { get; set; }

    public int FeatureId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Feature Feature { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
