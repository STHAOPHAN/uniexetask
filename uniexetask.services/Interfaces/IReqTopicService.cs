﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IReqTopicService
    {
        Task<IEnumerable<RegTopicForm>> GetAllReqTopic();
        Task<bool> CreateReqTopic(RegTopicForm reqTopic);
        Task<string> GetMaxTopicCode();
        Task<RegTopicForm?> GetReqTopicById(int id);
        Task<bool> UpdateReqTopic(RegTopicForm ReqTopics);
        Task<IEnumerable<int>> GetGroupByReqTopic();
        Task<List<RegTopicForm>> GetReqTopicByGroupId(int groupId);
        Task<bool> UpdateApproveTopic(int groupId);
        Task<Mentor> GetMentorGroupByUserId(int userId);
        Task<bool> RejectRegTopicFormAsync(int regTopicId, string? rejectionReason);
        Task<List<RegTopicForm>> GetReqTopicByDescription(string description);
        Task<List<RegTopicForm>> GetReqTopicByUserId(int userId);
        Task<List<RegTopicForm>> GetReqTopicByMentorId(int mentorId);
        Task<List<RegTopicForm>> GetAllReqTopicByMentorId(int mentorId);
    }
}
