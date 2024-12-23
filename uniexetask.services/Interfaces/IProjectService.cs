﻿using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjects();
        Task<IEnumerable<Project>> GetAllProjectsIsCurrentPeriod();
        Task<IEnumerable<Project>> GetAllDocuments();
        Task<Project?> GetProjectPendingByGroupAsync(Group group);
        Task<bool> UpdateProjectStatus(int projectId, string action);
        Task<Project?> GetProjectByStudentId(int studentId);
        Task<Project?> GetProjectByUserId(int userId);
        Task<Project> CreateProject(Project project);
        Task<IEnumerable<Project>> GetAllProjectsByGroupId(int groupId);
        Task<Project> GetProjectById(int projectId);
        Task<Project> GetProjectWithAllDataById(int projectId);
        Task<Project?> GetProjectWithTopicByGroupId(int groupId);
        System.Threading.Tasks.Task UpdateEndDurationEXE101();
        System.Threading.Tasks.Task UpdateEndDurationEXE201();
        Task<Boolean> CheckingProjectOfMentor(int projectId, int mentorId);
        //Task<bool> ContinueProject(int userId);
    }
}
