﻿using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ProjectService : IProjectService
    {
        public IUnitOfWork _unitOfWork;
        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Project>> GetAllProjects()

        {
            var project = await _unitOfWork.Projects.GetAllProjects();
            return project;
        }

        public async Task<Project?> GetProjectPendingByGroupAsync(Group group)
        {
            var project = await _unitOfWork.Projects.GetProjectPendingByGroupId(group.GroupId);
            return project;
        }

        public async Task<bool> UpdateProjectStatus(int projectId, string action)
        {
            var project = await _unitOfWork.Projects.GetByIDAsync(projectId);
            if (project == null) return false;

            if (action.Equals("accept")) 
            {
                project.Status = "Accepted";
                _unitOfWork.Save();
                return true;
            }
            else if (action.Equals("reject"))
            {
                project.Status = "Rejected";
                _unitOfWork.Save();
                return true;
            }
            else return false;
        }
    }
}
