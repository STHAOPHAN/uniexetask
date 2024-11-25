﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjects();
        Task<Project?> GetProjectPendingByGroupAsync(Group group);
        Task<bool> UpdateProjectStatus(int projectId, string action);
        Task<Project?> GetProjectByStudentId(int studentId);
        Task<Project?> GetProjectByUserId(int userId);
        Task<bool> CreateProject(Project project);
        Task<IEnumerable<Project>> GetAllProjectsByGroupId(int groupId);
        Task<Project> GetProjectById(int projectId);
        Task<Project> GetProjectWithAllDataById(int projectId);

    }
}
