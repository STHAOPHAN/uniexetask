﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IChatGroupService
    {
        Task<bool> AddMembersToChatGroupAsync(int groupId, List<string> emails);
        Task<bool> CreateChatGroupForGroup(Group group, int userId);
        Task<ChatGroup?> GetChatGroupByChatGroupId(int chatGroupId);
        Task<IEnumerable<ChatGroup>?> GetChatGroupByUserId(int userId, int chatGroupIndex, int limit, string keyword);
        Task<ChatGroup?> GetChatGroupWithUsersByChatGroupId(int chatGroupId);
        Task<ChatMessage?> GetLatestMessageInChatGroup(int chatGroupId);
        Task<IEnumerable<ChatMessage>?> GetMessagesInChatGroup(int chatGroupId, int messageIndex, int limit);
        Task<bool> RemoveMemberOutOfGroupChat(int userId, int chatGroupId);
        Task<ChatMessage?> SaveMessageAsync(int chatGroupId, int userId, string message);
        Task<bool> CreatePersonalChatGroup(int leaderId, int userId, string message);
        Task<ChatMessage> DeleteMessage(int userId, int messageId);
        Task<bool> AddUserToChatGroup(int userId, int chatGroupId);

    }
}
