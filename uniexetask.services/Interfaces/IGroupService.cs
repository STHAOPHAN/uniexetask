﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IGroupService
    {
        //Task<Group?> GetGroupWithProjectAsync(int groupId);
        Task<IEnumerable<object>> GetApprovedGroupsAsync();
        Task<IEnumerable<Group>> GetGroupAndSubject();
        Task<IEnumerable<Group>> GetGroupsAsync();
        Task<IEnumerable<Group>> GetAllGroup();
        Task<bool> CreateGroup(Group group);
        Task<Group> GetGroupById(int id);
        System.Threading.Tasks.Task AddMentorToGroup(int groupId, int mentorId);
    }
}
