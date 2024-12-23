﻿using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class ProjectScore
{
    public int ProjectScoreId { get; set; }

    public int CriteriaId { get; set; }

    public int ProjectId { get; set; }

    public double Score { get; set; }

    public string Comment { get; set; } = null!;

    public int ScoredBy { get; set; }

    public DateTime ScoringDate { get; set; }

    public virtual Criterion Criteria { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual User ScoredByNavigation { get; set; } = null!;
}
