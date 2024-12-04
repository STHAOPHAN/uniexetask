﻿using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ChatGroupService : IChatGroupService
    {
        public IUnitOfWork _unitOfWork;
        public ChatGroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddMembersToChatGroupAsync(int chatGroupId, List<string> emails)
        {
            var chatGroup = await _unitOfWork.ChatGroups.GetByIDAsync(chatGroupId);

            if (chatGroup == null || chatGroup.Type != nameof(ChatGroupType.Group))
                return false;
            foreach (var email in emails) 
            {
                var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
                if (user == null) continue;
                chatGroup.Users.Add(user);
            }
            _unitOfWork.Save();
            return true;
        }

        public async Task<bool> CreateChatGroupForGroup(core.Models.Group group, int userId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var chatGroup = new ChatGroup 
                {
                    ChatGroupName = group.GroupName,
                    ChatGroupAvatar = "https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg",
                    CreatedDate = DateTime.Now,
                    CreatedBy = userId,
                    OwnerId = userId,
                    GroupId = group.GroupId,
                    LatestActivity = DateTime.Now,
                    Type = nameof(ChatGroupType.Group)
                };
                await _unitOfWork.ChatGroups.InsertAsync(chatGroup);
                _unitOfWork.Save();
                var user = await _unitOfWork.Users.GetByIDAsync(userId);
                if (user == null) throw new Exception("User not found");
                chatGroup.Users.Add(user);
                _unitOfWork.ChatGroups.Update(chatGroup);
                _unitOfWork.Save();
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex) {
                _unitOfWork.Rollback();
                return false;
            }

        }

        public async Task<ChatGroup?> GetChatGroupByChatGroupId(int chatGroupId)
        {
            return await _unitOfWork.ChatGroups.GetByIDAsync(chatGroupId);
        }

        public async Task<IEnumerable<ChatGroup>?> GetChatGroupByUserId(int userId, int chatGroupIndex, int limit, string keyword)
        {
            var userWithChatGroups = await _unitOfWork.Users.GetUserWithChatGroupByUserIdAsyn(userId);
            if (userWithChatGroups == null || userWithChatGroups.ChatGroups == null) return null;
            var chatGroups = userWithChatGroups.ChatGroups;
            foreach (var chatgroup in chatGroups)
            {
                if (chatgroup.Type == nameof(ChatGroupType.Personal))
                {
                    var chatGroupWithUsers = await _unitOfWork.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(chatgroup.ChatGroupId);
                    if (chatGroupWithUsers == null || chatGroupWithUsers.Users == null) continue;
                    foreach (var user in chatGroupWithUsers.Users)
                    {
                        if (user.UserId == userId) continue;
                        chatgroup.ChatGroupName = user.FullName;
                        chatgroup.ChatGroupAvatar = user.Avatar;
                        break;
                    }
                }
            }
            var chatGroupsAfterPaging = chatGroups
                .Where(c => c.ChatGroupName.Contains(keyword))
                .Skip(chatGroupIndex * limit)
                .Take(limit)
                .OrderByDescending(e => e.LatestActivity)
                .ToList();
            return chatGroupsAfterPaging;
        }

        public async Task<ChatGroup?> GetChatGroupWithUsersByChatGroupId(int chatGroupId)
        {
            return await _unitOfWork.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(chatGroupId);
        }

        public async Task<ChatMessage?> GetLatestMessageInChatGroup(int chatGroupId)
        {
            return await _unitOfWork.ChatMessages.GetLatestMessageInChatGroup(chatGroupId);
        }

        public async Task<IEnumerable<ChatMessage>?> GetMessagesInChatGroup(int chatGroupId, int messageIndex, int limit)
        {
            var chatGroup = await _unitOfWork.ChatGroups.GetByIDAsync(chatGroupId);
            if (chatGroup == null) throw new Exception("Chat Group not found");
            var chatMessages = await _unitOfWork.ChatMessages.GetMessagesInChatGroup(chatGroupId, messageIndex, limit);
            if (chatMessages == null) return null;
            return chatMessages;
        }

        public System.Threading.Tasks.Task MarkAsReadAsync(string chatGroupId, string userId, int lastReadMessageId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveMemberOutOfGroupChat(int userId, int chatGroupId)
        {
            var chatGroup = await _unitOfWork.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(chatGroupId);
            if (chatGroup == null) return false;
            var userToRemove = chatGroup.Users.FirstOrDefault(u => u.UserId == userId);
            if (userToRemove != null)
            {
                chatGroup.Users.Remove(userToRemove);
                _unitOfWork.Save();
                return true;
            }
            return false;
        }

        public async Task<ChatMessage?> SaveMessageAsync(int chatGroupId, int userId, string message)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var chatGroup = await _unitOfWork.ChatGroups.GetByIDAsync(chatGroupId);
                if (chatGroup == null) throw new Exception("Chat Group not found");
                chatGroup.LatestActivity = DateTime.Now;
                _unitOfWork.ChatGroups.Update(chatGroup);
                var chatMessage = new ChatMessage
                {
                    ChatGroupId = chatGroupId,
                    UserId = userId,
                    MessageContent = message
                };
                await _unitOfWork.ChatMessages.InsertAsync(chatMessage);
                _unitOfWork.Save();
                _unitOfWork.Commit();
                return chatMessage;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CreatePersonalChatGroup(int contactedUserId, int contactUserId, string message)
        {
            var contactedUserExists = await _unitOfWork.Users.GetByIDAsync(contactedUserId);
            var contactUserExists = await _unitOfWork.Users.GetByIDAsync(contactUserId);

            if (contactedUserExists == null || contactUserExists == null)
            {
                throw new Exception("One or more users do not exist.");
            }

            var contactUserWithChatGroups = await _unitOfWork.Users.GetUserWithChatGroupByUserIdAsyn(contactUserId);
            var contactedUserWithChatGroups = await _unitOfWork.Users.GetUserWithChatGroupByUserIdAsyn(contactedUserId);

            var personalChatGroup = contactUserWithChatGroups.ChatGroups
                    .FirstOrDefault(userGroup => contactedUserWithChatGroups.ChatGroups
                        .Any(leaderGroup => leaderGroup.ChatGroupId == userGroup.ChatGroupId && leaderGroup.Type == nameof(ChatGroupType.Personal)));

            if (personalChatGroup != null)
            {
                await SaveMessageAsync(personalChatGroup.ChatGroupId, contactUserId, message);
                return true;
            }
            else 
            {
                try
                {
                    _unitOfWork.BeginTransaction();
                    var chatGroup = new ChatGroup
                    {
                        ChatGroupName = contactUserExists.FullName,
                        CreatedDate = DateTime.Now,
                        CreatedBy = contactUserId,
                        OwnerId = contactUserId,
                        LatestActivity = DateTime.Now,
                        Type = nameof(ChatGroupType.Personal),
                    };
                    await _unitOfWork.ChatGroups.InsertAsync(chatGroup);
                    _unitOfWork.Save();

                    chatGroup.Users.Add(contactUserExists);
                    chatGroup.Users.Add(contactedUserExists);
                    _unitOfWork.ChatGroups.Update(chatGroup);
                    _unitOfWork.Save();
                    _unitOfWork.Commit();
                    await SaveMessageAsync(chatGroup.ChatGroupId, contactUserId, message);
                    return true;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
