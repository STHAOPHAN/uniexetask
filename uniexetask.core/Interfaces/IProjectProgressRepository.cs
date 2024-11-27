﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IProjectProgressRepository : IGenericRepository<ProjectProgress>
    {
        Task<ProjectProgress?> GetProjectProgressByProjectId(int? projectId);

    }
}
