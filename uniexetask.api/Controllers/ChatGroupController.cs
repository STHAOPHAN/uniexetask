﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Security.Claims;
using uniexetask.services.Hubs;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/chat-groups")]
    [ApiController]
    public class ChatGroupController : ControllerBase
    {
        private readonly IChatGroupService _chatGroupService;
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly IMentorService _mentorService;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatGroupController(
            IChatGroupService chatGroupService, 
            IUserService userService, 
            IStudentService studentService, 
            IMentorService mentorService, 
            IMapper mapper, 
            IHubContext<ChatHub> hubContext)
        {
            _chatGroupService = chatGroupService;
            _userService = userService;
            _studentService = studentService;
            _mentorService = mentorService;
            _mapper = mapper;
            _hubContext = hubContext;

        }

        [HttpGet("user")]
        public async Task<IActionResult> GetChatGroupByUser(int chatGroupIndex = 0, int limit = 5, string keyword = "")
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<IEnumerable<ChatGroupResponse>> response = new ApiResponse<IEnumerable<ChatGroupResponse>>();
            List<ChatGroupResponse> list = new List<ChatGroupResponse>();
            try
            {
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                var chatgroups = await _chatGroupService.GetChatGroupByUserId(userId, chatGroupIndex, limit, keyword);
                if (chatgroups == null)
                {
                    throw new Exception("Chat group not found");
                }
                foreach (var chatgroup in chatgroups)
                {
                    Student? student = null;
                    int? userIdForPersonalChat = null;
                    if (chatgroup.Type == nameof(ChatGroupType.Personal))
                    {
                        var chatGroupWithUsers = await _chatGroupService.GetChatGroupWithUsersByChatGroupId(chatgroup.ChatGroupId);
                        if (chatGroupWithUsers != null && chatGroupWithUsers.Users != null && chatGroupWithUsers.Users.Count > 0)
                        {
                            foreach (var user in chatGroupWithUsers.Users)
                            {
                                if (user.UserId == userId) continue;
                                userIdForPersonalChat = user.UserId;
                                student = await _studentService.GetStudentByUserId(user.UserId);
                            }
                        }
                    }
                    var latestMessage = await _chatGroupService.GetLatestMessageInChatGroup(chatgroup.ChatGroupId);
                    string messageContent = latestMessage != null ? latestMessage.MessageContent : "[No Message]";
                    if (messageContent.Length > 50)
                    {
                        messageContent = messageContent.Substring(0, 50) + "...";
                    }
                    list.Add(new ChatGroupResponse
                    {
                        ChatGroup = chatgroup,
                        LatestMessage = messageContent,
                        SendDatetime = latestMessage != null ? latestMessage.SendDatetime : null,
                        Student = student != null ? _mapper.Map<StudentModel>(student) : null,
                        UserId = userIdForPersonalChat != null ? userIdForPersonalChat : null
                    });
                }
                response.Data = list;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("{chatGroupIdStr}/messages")]
        public async Task<IActionResult> GetMessagesChatGroup(string chatGroupIdStr, int messageIndex = 0, int limit = 5)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<IEnumerable<ChatMessageResponse>> response = new ApiResponse<IEnumerable<ChatMessageResponse>>();
            List<ChatMessageResponse> list = new List<ChatMessageResponse>();
            try
            {
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid Usser Id");
                }
                if (string.IsNullOrEmpty(chatGroupIdStr) || !int.TryParse(chatGroupIdStr, out int chatGroupId))
                {
                    throw new Exception("Invalid Chat Group Id");
                }
                var chatmessages = await _chatGroupService.GetMessagesInChatGroup(chatGroupId, messageIndex, limit);
                if (chatmessages == null)
                {
                    throw new Exception("Chat message not found");
                }
                foreach (var chatmessage in chatmessages)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var user = await _userService.GetUserById(chatmessage.UserId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    if (user == null) continue;

                    list.Add(new ChatMessageResponse
                    {
                        ChatMessage = chatmessage,
                        Avatar = user.Avatar,
                        SenderName = user.UserId == userId ? "You" : user.FullName
                    });
                }
                response.Data = list;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("{chatGroupIdStr}/members")]
        public async Task<IActionResult> GetMembersInChatGroup(string chatGroupIdStr)
        {
            ApiResponse<IEnumerable<MemberInChatGroupResponse>> response = new ApiResponse<IEnumerable<MemberInChatGroupResponse>>();
            List<MemberInChatGroupResponse> list = new List<MemberInChatGroupResponse>();
            try
            {
                if (string.IsNullOrEmpty(chatGroupIdStr) || !int.TryParse(chatGroupIdStr, out int chatGroupId))
                {
                    throw new Exception("Invalid Chat Group Id");
                }
                var chatGroup = await _chatGroupService.GetChatGroupWithUsersByChatGroupId(chatGroupId);
                if (chatGroup == null)
                {
                    throw new Exception("Chat group not found");
                }
                foreach (var user in chatGroup.Users)
                {
                    if (user == null) continue;
                    var mentor = await _mentorService.GetMentorByUserId(user.UserId);
                    list.Add(new MemberInChatGroupResponse
                    {
                        UserId = user.UserId,
                        FullName = user.FullName,
                        Email = user.Email,
                        IsOwner = (user.UserId == chatGroup.OwnerId) ? true : false,
                        IsMentor = mentor != null ? true : false
                    });
                }
                response.Data = list;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("add-members")]
        public async Task<IActionResult> AddMembers([FromBody] AddMembersModel request)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            if (request == null || request.GroupId <= 0 || request.Emails == null || request.Emails.Count == 0)
            {
                return BadRequest("Invalid request.");
            }

            var result = await _chatGroupService.AddMembersToChatGroupAsync(request.GroupId, request.Emails);

            if (result)
            {
                response.Data = "Members added successfully.";
                return Ok(response);
            }
            else
            {
                response.Success = false;
                response.ErrorMessage = "Failed to add members.";
                return BadRequest(response);
            }
        }

        [HttpPost("personal/contact")]
        public async Task<IActionResult> CreatePersonalChatGroup([FromBody] CreatePersonalChatGroupModal request)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (string.IsNullOrEmpty(request.ContactedUserId) || !int.TryParse(request.ContactedUserId, out int contactedUserId) ||
                    string.IsNullOrEmpty(request.ContactUserId) || !int.TryParse(request.ContactUserId, out int contactUserId) ||
                    string.IsNullOrEmpty(request.Message))
                {
                    throw new Exception("Invalid data.");
                }
                
                if (contactedUserId == contactUserId) throw new Exception("You can't contact yourself...");

                var result = await _chatGroupService.CreatePersonalChatGroup(contactedUserId, contactUserId, request.Message);

                if (result)
                {
                    response.Data = "Message has been sent, please go to Chat session to check..";
                    await _hubContext.Clients.All.SendAsync("UpdateChatGroupWhenContact", contactUserId);
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Failed to send message.");
                }
            }
            catch (Exception ex) 
            {
                    response.Success = false;
                    response.ErrorMessage = ex.Message;
                    return Ok(response);
            }
        }

        [HttpDelete("{messageIdStr}")]
        public async Task<IActionResult> DeleteMessage(string messageIdStr)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (string.IsNullOrEmpty(messageIdStr) || !int.TryParse(messageIdStr, out int messageId))
                {
                    throw new Exception("Invalid Meeting Schedule Id");
                }
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id");
                }
                var deletedMessage = await _chatGroupService.DeleteMessage(userId, messageId);
                if (deletedMessage != null)
                {
                    response.Data = "Message deleted successfully!";
                    var user = await _userService.GetUserById(deletedMessage.UserId);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    await _hubContext.Clients.All.SendAsync("UpdateMessageWhenChangeOccur", new ChatMessageResponse
                    {
                        ChatMessage = deletedMessage,
                        Avatar = user.Avatar,
                        SenderName = user.FullName
                    });
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    return Ok(response);
                }
                throw new Exception("Delete message failed.");

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }
    }
}
