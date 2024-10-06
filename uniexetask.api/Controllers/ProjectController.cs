﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        public IProjectService _projectService;
        public IMentorService _mentorService;
        public IGroupService _groupService;

        public ProjectController(IProjectService projectService, IGroupService groupService, IMentorService mentorService)
        {
            _projectService = projectService;
            _mentorService = mentorService;
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectList()
        {
            var projectsList = await _projectService.GetAllProjects();
            if (projectsList == null)
            {
                return NotFound();
            }
            List<ProjectListModel> projects = new List<ProjectListModel>();
            foreach (var project in projectsList)
            {
                if (project != null) projects.Add(new ProjectListModel
                {
                    TopicCode = project.TopicCode,
                    TopicName = project.TopicName,
                    Description = project.Description,
                    StartDate = project.StartDate
                });
            }
            ApiResponse<IEnumerable<ProjectListModel>> response = new ApiResponse<IEnumerable<ProjectListModel>>();
            response.Data = projects;
            return Ok(response);
        }

        [Authorize(Roles = "4")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetProjectsPendingWithMentor()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId) || string.IsNullOrEmpty(userRole) || !userRole.Equals("4"))
            {
                return NotFound();
            }
            var mentor = await _mentorService.GetMentorWithGroupAsync(userId);
            if (mentor == null) return NotFound();
            Project? project = new Project();
            List<ProjectPendingModel> projects = new List<ProjectPendingModel>();
            foreach (var item in mentor.Groups)
            {
                project = await _projectService.GetProjectPendingByGroupAsync(item);
                if (project != null) projects.Add(new ProjectPendingModel
                {
                    id = project.ProjectId.ToString(),
                    GroupName = item.GroupName,
                    Topic = project.TopicName,
                    Description = project.Description,
                    Status = project.Status
                });
            }
            ApiResponse<IEnumerable<ProjectPendingModel>> response = new ApiResponse<IEnumerable<ProjectPendingModel>>();
            response.Data = projects;
            return Ok(response);
        }

        [Authorize(Roles = "4")]
        [HttpPost("projects/{projectId}/update-status")]
        public async Task<IActionResult> UpdateProjectStatus(string projectIdStr, [FromBody] string action)
        {
            if (string.IsNullOrEmpty(projectIdStr) || !int.TryParse(projectIdStr, out int projectId) || string.IsNullOrEmpty(action))
            {
                return NotFound();
            }
            var result = await _projectService.UpdateProjectStatus(projectId, action);
            if (result) return Ok("Done");
            else return BadRequest();
        }
    }
}
