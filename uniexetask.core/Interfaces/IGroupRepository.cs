﻿using uniexetask.core.Models;
using uniexetask.core.Models.Enums;

namespace uniexetask.core.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        Task<Group?> GetGroupWithProjectAsync(int groupId);
        Task<Group?> GetGroupWithSubjectAsync(int groupId);
        Task<IEnumerable<Group>> GetHasNoMentorGroupsWithGroupMembersAndStudent(SubjectType subjectType);
        Task<IEnumerable<Group>> GetApprovedGroupsWithGroupMembersAndStudent();
        Task<Mentor?> GetMentorInGroup(int groupId);
        Task<IEnumerable<Group>> SearchGroupsByGroupNameAsync(int mentorId, string query);
        Task<IEnumerable<Group>> GetCurrentPeriodGroupsWithMembersAndMentor();
        System.Threading.Tasks.Task RemoveMentorFromGroup(int groupId);
    }
}
